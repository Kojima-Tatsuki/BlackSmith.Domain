#!/usr/bin/env python3
"""Simple Markdown link checker without external dependencies."""

# Markdown ドキュメント内のリンク切れを検出する簡易スクリプト。
# 外部ライブラリへ依存せず、標準ライブラリだけで動作することを目的としている。

from __future__ import annotations

import argparse
import concurrent.futures
import html
from bisect import bisect_right
from pathlib import Path
import re
import sys
from typing import Dict, Iterable, List, Optional, Sequence, Tuple
import urllib.error
import urllib.parse
import urllib.request

# HTTP レスポンスの成功範囲。
HTTP_OK_RANGE = range(200, 400)
# Markdown からリンク文字列を抽出するための正規表現群。
INLINE_LINK_RE = re.compile(r"(?<!!)\[[^\]]+\]\(([^)\s]+)\)")
REFERENCE_DEF_RE = re.compile(r"^\s*\[[^\]]+\]:\s*(\S+)", re.MULTILINE)
BARE_URL_RE = re.compile(r"(?P<url>https?://[^\s)]+)")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Check Markdown links for validity.")
    parser.add_argument(
        "paths",
        nargs="*",
        default=["Assets/Plugins/BlackSmith.Domain/docs"],
        help="Directories or Markdown files to scan.",
    )
    parser.add_argument(
        "--root",
        default=Path.cwd(),
        type=Path,
        help="Repository root used to resolve relative links (default: current working directory).",
    )
    parser.add_argument(
        "--timeout",
        type=float,
        default=10.0,
        help="HTTP request timeout in seconds.",
    )
    parser.add_argument(
        "--max-workers",
        type=int,
        default=8,
        help="Maximum number of concurrent HTTP checks.",
    )
    return parser.parse_args()


def gather_markdown_files(paths: Iterable[str]) -> List[Path]:
    """Resolve directories/files to Markdown ファイルの一覧にする。"""
    # ディレクトリが指定された場合は再帰的に .md を列挙し、重複は排除する。
    files: List[Path] = []
    for item in paths:
        path = Path(item)
        if path.is_dir():
            files.extend(path.rglob("*.md"))
        elif path.is_file() and path.suffix.lower() == ".md":
            files.append(path)
    return sorted(set(files))


def extract_links(markdown_text: str) -> List[Tuple[str, str, int]]:
    """Markdown からリンク候補となる文字列を集める。"""
    # インライン記法、参照記法、裸の URL いずれも対象とし、出現行も保持する。
    newline_positions = _compute_newline_positions(markdown_text)
    occurrences: List[Tuple[str, str, int]] = []

    for match in INLINE_LINK_RE.finditer(markdown_text):
        raw = match.group(1)
        occurrences.append((raw, normalize_link(raw), _line_for_index(newline_positions, match.start(1))))

    for match in REFERENCE_DEF_RE.finditer(markdown_text):
        raw = match.group(1)
        occurrences.append((raw, normalize_link(raw), _line_for_index(newline_positions, match.start(1))))

    for match in BARE_URL_RE.finditer(markdown_text):
        raw = match.group("url")
        occurrences.append((raw, normalize_link(raw), _line_for_index(newline_positions, match.start("url"))))

    return occurrences


def _compute_newline_positions(text: str) -> List[int]:
    """改行のインデックス一覧を構築し、行番号計算を高速化する。"""
    return [index for index, char in enumerate(text) if char == "\n"]


def _line_for_index(newlines: Sequence[int], index: int) -> int:
    """文字インデックスから 1 始まりの行番号を求める。"""
    return bisect_right(newlines, index) + 1


def normalize_link(link: str) -> str:
    # HTML エスケープを戻し、前後の空白を削除して正規化する。
    return html.unescape(link.strip())


def is_ignored(link: str) -> bool:
    """mailto などチェック不要なリンクを除外する。"""
    # 空文字、ページ内アンカー、メールアドレスなどはチェック対象外とする。
    lowered = link.lower()
    return (
        not link
        or lowered.startswith("#")
        or lowered.startswith("mailto:")
        or lowered.startswith("tel:")
        or lowered.startswith("data:")
    )


def check_local_link(base_file: Path, root: Path, link: str) -> Optional[str]:
    """ローカルファイルへのリンクとアンカーを検証する。"""
    # 相対パスは参照元ファイルから解決し、リポジトリ外への参照はエラーとする。
    target_part, _, anchor = link.partition("#")
    target_path = Path(target_part) if target_part else Path(base_file.name)
    if not target_path.is_absolute():
        resolved = (base_file.parent / target_path).resolve()
    else:
        resolved = target_path.resolve()

    try:
        resolved.relative_to(root.resolve())
    except ValueError:
        return f"points outside repository: {resolved}"

    if not resolved.exists():
        return f"missing file: {target_part or base_file.name}"

    if anchor:
        if not anchor_exists(resolved, anchor):
            return f"missing anchor '#{anchor}' in {resolved.relative_to(root)}"

    return None


def anchor_exists(path: Path, anchor: str) -> bool:
    """Markdown の見出しから生成されるアンカーを探索する。"""
    # GitHub 等のスラグ生成規則を簡易再現した slugify と比較する。
    if path.suffix.lower() != ".md":
        return True
    slug = anchor.lower()
    anchor_re = re.compile(r"^#+\s+(.+)$")
    try:
        with path.open("r", encoding="utf-8") as handle:
            for line in handle:
                match = anchor_re.match(line.strip())
                if match:
                    candidate = slugify(match.group(1))
                    if candidate == slug:
                        return True
    except OSError:
        return False
    return False


def slugify(text: str) -> str:
    # 見出しの文字列から簡易的にアンカー名を推測する。
    cleaned = re.sub(r"[\\s]+", "-", text.strip().lower())
    cleaned = re.sub(r"[^a-z0-9\-]", "", cleaned)
    return cleaned


def check_http_link(link: str, timeout: float) -> Optional[str]:
    """HTTP/HTTPS リンクが有効かどうか確認する。"""
    # まず HEAD で応答確認し、許可されない場合のみ GET へフォールバックする。
    url = link
    try:
        return perform_request(url, timeout, method="HEAD")
    except urllib.error.HTTPError as error:
        if error.code in (400, 403, 405, 501):
            return perform_request(url, timeout, method="GET")
        return f"HTTP {error.code}: {error.reason}"
    except urllib.error.URLError as error:
        return f"network error: {error.reason}"


def perform_request(url: str, timeout: float, method: str) -> Optional[str]:
    # User-Agent を指定しないと 403 を返すサイトがあるため、簡易 UA を付与する。
    request = urllib.request.Request(url, method=method, headers={"User-Agent": "link-checker/1.0"})
    with urllib.request.urlopen(request, timeout=timeout) as response:
        status = getattr(response, "status", None)
        if status is None:
            return None
        if status in HTTP_OK_RANGE:
            return None
        return f"unexpected status {status}"


def classify_link(link: str) -> str:
    parsed = urllib.parse.urlparse(link)
    if parsed.scheme in ("http", "https"):
        return "http"
    if parsed.scheme:
        return "other"
    return "local"


def check_links(files: Iterable[Path], root: Path, timeout: float, max_workers: int) -> List[Tuple[Path, str, str, int]]:
    """全リンクを走査し、問題があれば (ファイル, 元リンク, 理由, 行) を返す。"""
    # HTTP リンクはまとめて並列処理し、ローカルリンクは逐次検証する。
    broken: List[Tuple[Path, str, str, int]] = []
    http_links: Dict[str, List[Tuple[Path, str, int]]] = {}

    for file_path in files:
        try:
            content = file_path.read_text(encoding="utf-8")
        except OSError as error:
            broken.append((file_path, "", f"unable to read file: {error}"))
            continue

        for raw_link, link, line in extract_links(content):
            if is_ignored(link):
                continue
            kind = classify_link(link)
            if kind == "http":
                http_links.setdefault(link, []).append((file_path, raw_link, line))
            elif kind == "local":
                reason = check_local_link(file_path, root, link)
                if reason:
                    broken.append((file_path, raw_link, reason, line))
            else:
                broken.append((file_path, raw_link, f"unsupported scheme in link '{link}'", line))

    broken.extend(check_http_links_concurrently(http_links, timeout, max_workers))
    return broken


def check_http_links_concurrently(http_links: Dict[str, List[Tuple[Path, str, int]]], timeout: float, max_workers: int) -> List[Tuple[Path, str, str, int]]:
    """HTTP リンクはスレッドプールで並列に検査する。"""
    # 同じ URL が複数回登場する場合は一度だけアクセスし、結果を全ての出現箇所へ反映する。
    broken: List[Tuple[Path, str, str, int]] = []
    results: Dict[str, Optional[str]] = {}

    def task(url: str) -> Tuple[str, Optional[str]]:
        return url, check_http_link(url, timeout)

    with concurrent.futures.ThreadPoolExecutor(max_workers=max_workers) as executor:
        future_map = {executor.submit(task, url): url for url in http_links}
        for future in concurrent.futures.as_completed(future_map):
            url, message = future.result()
            results[url] = message

    for url, locations in http_links.items():
        message = results.get(url)
        if message:
            for file_path, raw_link, line in locations:
                broken.append((file_path, raw_link, message, line))

    return broken


def main() -> int:
    args = parse_args()
    root = args.root.resolve()
    files = gather_markdown_files(args.paths)
    if not files:
        print("No Markdown files found in the specified paths.")
        return 0

    broken = check_links(files, root, args.timeout, args.max_workers)
    if broken:
        print("Broken links detected:")
        for file_path, raw_link, reason, line in broken:
            rel_path = file_path.resolve().relative_to(root)
            print(f"- {rel_path}:{line}: '{raw_link}' -> {reason}")
        return 1

    print(f"Checked {len(files)} Markdown files; all links look good.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
