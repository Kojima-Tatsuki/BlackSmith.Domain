using System;

namespace BlackSmith.Domain.Character
{
    /// <summary>�v���C���[��G���܂ނ��ׂẴL�����N�^�[����ӂɒ�߂鎯�ʎq</summary>
    public class CharacterID : BasicID
    {
        protected override string Prefix => "Character-";

        public CharacterID() : base() { }

        public CharacterID(string id) : base(id) { }
    }
}