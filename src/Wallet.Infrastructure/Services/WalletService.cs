using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wallet.Application.Contracts;
using Wallet.Application.Dtos.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Persitstance;
using Wallet.Resources;

namespace Wallet.Infrastructure.Services;

public class WalletService : BaseService<WalletService>, IWalletManageService
{
    private readonly ApplicationDbContext _context;
    public WalletService(IMapper mapper,
        ILogger<WalletService> logger,
        ApplicationDbContext context) : base(mapper, logger)
    {
        _context = context;
    }

    public async Task<ResponseDto> CashIn(string mobileNumber)
    {
        ResponseDto response = new()
        {
            ResponseCode = ResponseCodes.FailedToProcess,
            ResponseMessage = Resource.Failed
        };

        try
        {
            decimal amount = 1000;
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.MobileNumber.Equals(mobileNumber));
            if (user == null)
            {
                response.ResponseMessage = Resource.SenderMobileNotFound;
                response.ResponseCode = ResponseCodes.FailedToProcess;
                serializedResponse = JsonConvert.SerializeObject(response);
                Logger.LogError(serializedResponse);
                return response;
            }
            
            
            // Add the signUp bouunce to customer 
            user.Balance += amount;

            // Create a new transaction record in the database
            Transaction transfer = new()
            {   
                RecipientId = user.Id,
                Amount = amount,
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(transfer);
            var isSaved = await _context.SaveChangesAsync();
            if (isSaved > 0)
            {
                response.ResponseMessage = Resource.Sucess;
                response.ResponseCode = ResponseCodes.ProcessedSuccessfully;
                return response;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex.StackTrace);
            response.ResponseMessage = Resource.GeneralError;
            response.ResponseCode = ResponseCodes.GeneralError;
            serializedResponse = JsonConvert.SerializeObject(response);
            Logger.LogError(serializedResponse);
        }
        return response;
    }

    public async Task<BalanceResponseDto> CheckBalance(string userMobile)
    {
        BalanceResponseDto response = new()
        {
            ResponseCode = ResponseCodes.FailedToProcess,
            ResponseMessage = Resource.Failed,
            Balance = decimal.Zero
        };

        try
        {
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.MobileNumber.Equals(userMobile));
            if (user == null)
            {
                response.ResponseMessage = Resource.UserNotFound;
                response.ResponseCode = ResponseCodes.FailedToProcess;
                serializedResponse = JsonConvert.SerializeObject(response);
                Logger.LogError(serializedResponse);
                return response;
            }

            response.ResponseMessage = Resource.Sucess;
            response.ResponseCode = ResponseCodes.ProcessedSuccessfully;
            response.Balance = user.Balance;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex.StackTrace);
            response.ResponseMessage = Resource.GeneralError;
            response.ResponseCode = ResponseCodes.GeneralError;
            serializedResponse = JsonConvert.SerializeObject(response);
            Logger.LogError(serializedResponse);
        }

        return response;
    }

    public async Task<ResponseDto> TransferToOthers(TransferRequestDto transferRequest)
    {
        using var transaction = _context.Database.BeginTransaction();
        ResponseDto response = new()
        {
            ResponseCode = ResponseCodes.FailedToProcess,
            ResponseMessage = Resource.Failed
        };

        try
        {
            var amount  = transferRequest.Amount;
            var sender = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.MobileNumber.Equals(transferRequest.SenderMobileNumber));
            if(sender == null )
            {
                response.ResponseMessage = Resource.SenderMobileNotFound;
                response.ResponseCode = ResponseCodes.FailedToProcess;
                await transaction.RollbackAsync();
                return response;
            }
            var remaingBalance = sender.Balance - amount;
            if (remaingBalance < 0)
            {
                response.ResponseMessage = Resource.Failed;
                response.ResponseCode = ResponseCodes.InsuufiecientBalance;
                await transaction.RollbackAsync();
                return response;
            }

            var recipient = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.MobileNumber.Equals(transferRequest.RecipientMobileNumber));
            if (recipient == null)
            {
                response.ResponseMessage = Resource.RecipientMobileISNotFound;
                response.ResponseCode = ResponseCodes.FailedToProcess;
                await transaction.RollbackAsync();
                return response;
            }
            // Deduct the transfer amount from the sender's balance
            sender.Balance -=amount; 
            // Add the transfer amount to the recipient's balance
            recipient.Balance += amount;

            // Create a new transaction record in the database
            Transaction transfer = new ()
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Amount = amount,
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(transfer);
           var isSaved =  await _context.SaveChangesAsync();
            if (isSaved>0)
            {
                await transaction.CommitAsync();
                response.ResponseMessage = Resource.Sucess;
                response.ResponseCode = ResponseCodes.ProcessedSuccessfully;
                return response;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex.StackTrace);
            response.ResponseMessage = Resource.GeneralError;
            response.ResponseCode = ResponseCodes.GeneralError;
            await transaction.RollbackAsync();
        }
        return response;
    }
}
