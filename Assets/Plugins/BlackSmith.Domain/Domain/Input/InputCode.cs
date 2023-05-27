using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Input
{
    /// <summary>
    /// 入力情報と時間を格納するデータ構造体
    /// </summary>
    public class InputCode
    {
        public InputType Value { get; }

        public DateTime DateTime { get; }

        /// <summary>
        /// 入力情報と時間を格納する
        /// </summary>
        /// <param name="type">入力されたキー</param>
        /// <param name="dateTime">入力された時間</param>
        public InputCode(InputType type, DateTime dateTime)
        {
            Value = type;
            DateTime = dateTime;
        }

        public override string ToString() => Enum.GetName(typeof(InputType), Value);
    }

    public enum InputType
    {
        None = 0,
        Return,
        BackSpace,
        Space,
        Escape,
        Shift,
        Tab,
        Cntrol,
        Right,
        Left,
        Up,
        Down,
        A = 97,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
    }
}