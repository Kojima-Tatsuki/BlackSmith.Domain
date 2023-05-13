using BlackSmith.Domain.Input;
using System.Collections.Generic;

namespace BlackSmith.Usecase.Interface
{
    public interface ITranslateInputToActionUsecase
    {
        ActionCode? ExtractionActionCode(IReadOnlyCollection<InputCode> code);

        MoveActionCode? ExtractionMoveActionCode(IReadOnlyCollection<InputCode> codes);
    }
}