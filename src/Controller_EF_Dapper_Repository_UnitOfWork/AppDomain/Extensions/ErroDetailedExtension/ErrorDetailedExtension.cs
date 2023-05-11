using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;

namespace Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Extensions.ErroDetailedExtension
{
    //----------------------------------------------------------------------------------------------
    // Extensao de apoio a notifcação de erros padrao do Asp.net
    // Esta extensao cria um Dictioray com a lista de todos os erros validados
    //----------------------------------------------------------------------------------------------

    public static class ErrorDetailExtension
    {
        public static Dictionary<string, string[]> ConvertToErrorDetails(this IReadOnlyCollection<Notification> notifications)
        {
            return notifications
                            .GroupBy(g => g.Key)
                            .ToDictionary(g => g.Key, g => g.Select(x => x.Message)
                            .ToArray());
        }

        public static Dictionary<string, string[]> ConvertToErrorDetails(this IEnumerable<IdentityError> error)
        {
            var dictionary = new Dictionary<string, string[]>();
            dictionary.Add("Error", error.Select(e => e.Description).ToArray());
            return dictionary;

            //        return error
            //               .GroupBy(g => g.Code)
            //               .ToDictionary(g => g.Key, g => g.Select(x => x.Description)
            //               .ToArray());
        }
    }
}