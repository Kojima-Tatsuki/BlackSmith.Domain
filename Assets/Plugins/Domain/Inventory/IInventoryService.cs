using BlackSmith.Domain.Item;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Inventory
{
    using Item;
    using Currency;

    public interface IInventoryService : IInventoryService<IItem>, IInventoryStateViewable<IItem> { }

    public interface IInventoryStateViewable : IInventoryStateViewable<IItem> { }

    /// <summary>
    /// �C���x���g���̑��������ۂɎg��Interface
    /// </summary>
    /// <typeparam name="T">�C���x���g���Ɋi�[����A�C�e���̌^</typeparam>
    public interface IInventoryService<T> : IInventoryStateViewable<T> where T : IItem
    {
        /// <summary>
        /// �A�C�e����ǉ�����
        /// </summary>
        /// <param name="item">�ǉ�����A�C�e��</param>
        /// <param name="count">�ǉ������</param>
        /// <returns>�ǉ������A�C�e��</returns>
        T AddItem(T item, int count = 1!);

        /// <summary>
        /// �A�C�e������菜��
        /// </summary>
        /// <param name="item">��菜���A�C�e��</param>
        /// <param name="count">��菜����</param>
        /// <returns>��菜�����A�C�e��</returns>
        T RemoveItem(T item, int count = 1!);

        /// <summary>
        /// �A�C�e�����C���x���g�����ɂ��邩��Ԃ�
        /// </summary>
        /// <param name="item">�T������A�C�e��</param>
        /// <returns>���݂���ΐ^��Ԃ�</returns>
        bool IsContain(T item);

        bool IsAddable(T item, int count = 1!);
    }

    /// <summary>1���������삪�s���Ȃ��C���x���g��</summary>
    public interface IOneByInventoryService<T>: IInventoryStateViewable<T> where T: IItem
    {
        /// <summary>
        /// �A�C�e����ǉ�����
        /// </summary>
        /// <param name="item">�ǉ�����A�C�e��</param>
        /// <returns>�ǉ������A�C�e��</returns>
        T AddItem(T item);

        /// <summary>
        /// �A�C�e������菜��
        /// </summary>
        /// <param name="item">��菜���A�C�e��</param>
        /// <returns>��菜�����A�C�e��</returns>
        T RemoveItem(T item);

        /// <summary>
        /// �A�C�e�����C���x���g�����ɂ��邩��Ԃ�
        /// </summary>
        /// <param name="item">�T������A�C�e��</param>
        /// <returns>���݂���ΐ^��Ԃ�</returns>
        bool IsContain(T item);

        bool IsAddable(T item);
    }

    /// <summary>
    /// �C���x���g���̏���\���\
    /// </summary>
    /// <typeparam name="T">�C���x���g���Ɋi�[����A�C�e���̌^</typeparam>
    public interface IInventoryStateViewable<T> where T : IItem
    {
        /// <summary>
        /// �i�[����Ă��邷�ׂẴA�C�e���Ƃ��̌���Ԃ�
        /// </summary>
        /// <returns>�A�C�e���Ƃ��̌��̎����^</returns>
        IReadOnlyDictionary<T, int> GetInventory();

        /// <summary>
        /// �i�[����Ă���A�C�e�������ׂĕԂ�
        /// </summary>
        /// <returns>�i�[����Ă���A�C�e��</returns>
        IReadOnlyCollection<T> GetContainItems();

        /// <summary>
        /// �Ώۂ̃A�C�e���̏�������Ԃ�
        /// </summary>
        /// <param name="item">�Ώۂ̃A�C�e��</param>
        /// <returns>������</returns>
        int GetContainItemCount(T item);
    }

    /// <summary>
    /// ���K������
    /// </summary>
    public interface IWallet
    {
        public void AdditionMoney(Currency money);

        public void SubtractMoney(Currency money);

        /// <summary>
        /// ��������Ԃ�
        /// </summary>
        /// <returns>������</returns>
        IReadOnlyCollection<Currency> GetMoney();

        Currency GetMoney(CurrencyType type);

        bool IsContainType(CurrencyType type);
    }
}