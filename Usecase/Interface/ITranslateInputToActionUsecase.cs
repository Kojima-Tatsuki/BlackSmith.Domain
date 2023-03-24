using BlackSmith.Domain.Input;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Usecase.Input
{
    public interface ITranslateInputToActionUsecase
    {
        ActionCode? ExtractionActionCode(IReadOnlyCollection<InputCode> code);

        MoveActionCode? ExtractionMoveActionCode(IReadOnlyCollection<InputCode> codes);
    }
}