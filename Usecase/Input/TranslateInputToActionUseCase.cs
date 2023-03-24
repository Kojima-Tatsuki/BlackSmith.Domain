using BlackSmith.Domain.Input;
using System.Collections.Generic;

namespace BlackSmith.Usecase.Input
{
    public class TranslateInputToActionUsecase : ITranslateInputToActionUsecase
    {
        public ActionCode ExtractionActionCode(IReadOnlyCollection<InputCode> code)
        {
            return new ActionCode(new ActionName("TestAction"));
        }

        public MoveActionCode ExtractionMoveActionCode(IReadOnlyCollection<InputCode> codes)
        {
            return new MoveActionCode(new ActionName("TestMoveAction"), MoveDirection.None);
        }
    }
}