using Blazored.LocalStorage;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Infrastructure; 

namespace Client.Services
{
    public class SupportService
    {
        private const string key = "support_";
        private readonly string key_all = key + "all";

        private readonly ILocalStorageService _localStorageService;

        public SupportService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> ContainsItem(Guid id) => await _localStorageService.ContainKeyAsync(key + id);

        public async Task<List<SupportApplicationMainInfoVM>> GetAllItems(string searchString, SupportApplicationType? type)
        {
            if (await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<SupportApplicationMainInfoVM>>(key_all);
                if (type != null)
                    return allItems.Where(c => c.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) && c.Type == type).ToList();
                else
                    return allItems.Where(c => c.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                return new();
            }
        }

        public async Task<SupportApplicationFullInfoVM> GetApplication(Guid id)
        {
            Console.WriteLine(await _localStorageService.ContainKeyAsync(key + id));
            return await _localStorageService.GetItemAsync<SupportApplicationFullInfoVM>(key + id);
        }

        public async Task CreateNewItem(SupportApplicationFullInfoVM item)
        {
            if (await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<SupportApplicationFullInfoVM>>(key_all);
                await _localStorageService.SetItemAsync(key_all, allItems.Prepend(item));
            }
            else
                await _localStorageService.SetItemAsync(key_all, new List<SupportApplicationFullInfoVM>() { item });
            if (!await _localStorageService.ContainKeyAsync(key + item.Id))
            {
                await _localStorageService.SetItemAsync(key + item.Id, item);
            }
        }

        public async Task<bool> AddNewComment(CommentVM comment, Guid applicationId, Guid? parentId)
        {
            if(await _localStorageService.ContainKeyAsync(key + applicationId))
            {
                var application = await _localStorageService.GetItemAsync<SupportApplicationFullInfoVM>(key + applicationId);
                if(parentId != null)
                {
                    var parentComment = application.Comments.Find(parentId.Value);
                    if(parentComment != null)
                    {
                        parentComment.Children.Add(comment);
                        await _localStorageService.SetItemAsync(key + applicationId, application);
                        return true;
                    }
                    return false;
                }
                application.Comments.Add(comment);
                await _localStorageService.SetItemAsync(key + applicationId, application);
                return true;
            }
            return false;
        }
    }
}
