using System;

namespace BlackSmith.Domain.Character
{
    /// <summary>�v���C���[��G���܂ނ��ׂẴL�����N�^�[����ӂɒ�߂鎯�ʎq</summary>
    public class CharacterID : BasicID
    {
        internal CharacterID(Guid id) : base(id)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}