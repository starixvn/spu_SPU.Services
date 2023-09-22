namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Interface
{
    using Fanex.NAP.DCS.Model;
    using Fanex.Oneyed.Infrastructure.Models;
    public interface IRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);
    }
}
