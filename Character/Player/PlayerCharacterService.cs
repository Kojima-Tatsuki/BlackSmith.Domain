using System;
using System.Collections.Generic;
using BlackSmith.Domain.CharacterObjects;

namespace BlackSmith.Domain.Player
{
    internal class PlayerCharacterService
    {
        // �L�����N�^�[�ɑ΂��đΏۂ̑����𑕔�������
        // ���ɑ������Ă����ꍇ�ɂ́A�G���[����������
        internal void Equip(PlayerEntity player, Equipment equip)
        {
            /*
            �v���C���[�̑������Ɏw��̑����������ł���Ȃ�A��������
            ���̎��A�����ł��邩�̏����̓v���C���[�̃h���C���I�u�W�F�N�g���ɋL�ڂ��ׂ�
            -> �����ł́A�����ł��邩 �Ƃ��� bool ��Ԃ��֐������s���ׂ�

            �����������A�v���C���[�̃X�e�[�^�X�́A���̑����̉e�����󂯂�

            �܂��A���������ۂɐV���Ƀv���C���[�ɉ�������d�ʂɂ����
            �v���C���[�Ƀf�o�t�������邩�̊m�F���s��

            �h���C���T�[�r�X�̌`�Ŏ������悤�Ƃ��Ă���̂́A
            �v���C���[�̑��������|�W�g���ɓ���̂��A
            �v���C���[�̃G���e�B�e�B�ɓ���̂������肾����ł���

            �v���C���[�̃G���e�B�e�B������̏�Ԉُ��C���x���g�����A�������Ă��鑕���ȂǁA
            ���ׂĂ̏���ێ����Ă���΂����̂����A����̓f�[�^�𕪂���Ƃ������Ƃ�����A
            ���܂�]�܂����Ȃ��\��������
            */
        }
    }
}
