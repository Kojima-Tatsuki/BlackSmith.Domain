using BlackSmith.Domain.Input;

namespace BlackSmith.Usecase.Interface
{
    public interface ITranslateInputToActionUsecase
    {
        ActionCode? ExtractionActionCode(IReadOnlyCollection<InputCode> code);

        MoveActionCode? ExtractionMoveActionCode(IReadOnlyCollection<InputCode> codes);
    }
}