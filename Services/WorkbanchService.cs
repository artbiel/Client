using Blazored.LocalStorage;
using Client.Infrastructure;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Services
{
    public class WorkbanchService
    {
        private const string key = "workbanch_";
        private readonly string key_all = key + "all";

        private readonly ILocalStorageService _localStorageService;

        public WorkbanchService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> ContainsItem(Guid id) => await _localStorageService.ContainKeyAsync(key + id);

        public async Task<List<WorkbanchItemVM>> GetAllItems(string searchString/*, WorkbanchItemType? type*/)
        {
            if (await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<WorkbanchItemVM>>(key_all);
                //if (type != null)
                //    return allItems.Where(c => c.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) && c.Type == type).ToList();
                //else
                return allItems.Where(c => c.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                return new();
            }
        }

        public async Task<T> GetItem<T>(Guid id) where T : BaseWBItem
        {
            
            var item = await _localStorageService.GetItemAsync<T>(key + id);
            if(item is CourseFullInfoVM)
            (item as CourseFullInfoVM).ImgSrc = "https://cloudhostingasp.net/wp-content/uploads/2018/06/flat1000x1000075f.u1.jpg";
            return item;
        }

        public async Task CreateNewItem<T>(WorkbanchItemVM wbitem, T item) where T : BaseModel
        {
            if (await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<WorkbanchItemVM>>(key_all);
                await _localStorageService.SetItemAsync(key_all, allItems.Prepend(wbitem));
            }
            else
                await _localStorageService.SetItemAsync(key_all, new List<WorkbanchItemVM>() { wbitem });
            if (!await _localStorageService.ContainKeyAsync(key + item.Id))
            {
                await _localStorageService.SetItemAsync(key + item.Id, item);
            }
        }

        public async Task DeleteItem(Guid id)
        {
            if (await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<WorkbanchItemVM>>(key_all);
                var listItem = allItems.FirstOrDefault(i => i.Id == id);
                if (listItem != null)
                {
                    allItems.Remove(listItem);
                    await _localStorageService.SetItemAsync(key_all, allItems);
                }
            }
            if (await _localStorageService.ContainKeyAsync(key + id))
            {
                await _localStorageService.RemoveItemAsync(key + id);
            }
        }

        public async Task<bool> EditTitle<T>(Guid id, string title, Guid? courseId) where T : BaseWBItem
        {
            if (await _localStorageService.ContainKeyAsync(key + id) && await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<WorkbanchItemVM>>(key_all);
                var wbitem = allItems.FirstOrDefault(i => i.Id == id);
                if (wbitem != null)
                {
                    var item = await _localStorageService.GetItemAsync<T>(key + id);
                    item.Title = title;
                    await _localStorageService.SetItemAsync(key + item.Id, item);
                    wbitem.Title = title;
                    await _localStorageService.SetItemAsync(key_all, allItems);
                    if (courseId != null)
                    {
                        var course = await _localStorageService.GetItemAsync<CourseFullInfoVM>(key + courseId);
                        course.RootRecord.Find(id).Title = title;
                        Console.WriteLine(course.RootRecord.Find(id).Title);
                        await _localStorageService.SetItemAsync(key + courseId, course);
                    }
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> EditArticleContent(Guid id, string content)
        {
            if (await _localStorageService.ContainKeyAsync(key + id))
            {
                var item = await _localStorageService.GetItemAsync<WorkbanchArticleVM>(key + id);
                item.Content = content;
                await _localStorageService.SetItemAsync(key + item.Id, item);
                return true;
            }
            return false;
        }

        public async Task<bool> EditCourse(CourseFullInfoVM course)
        {
            if (await _localStorageService.ContainKeyAsync(key + course.Id) && await _localStorageService.ContainKeyAsync(key_all))
            {
                var allItems = await _localStorageService.GetItemAsync<List<WorkbanchItemVM>>(key_all);
                var wbitem = allItems.FirstOrDefault(i => i.Id == course.Id);
                if (wbitem != null)
                {
                    await _localStorageService.SetItemAsync(key + course.Id, course);
                    wbitem.Title = course.Title;
                    await _localStorageService.SetItemAsync(key_all, allItems);
                    return true;
                }
            }
            return false;
        }

        private List<WorkbanchItemVM> GetExample() => new List<WorkbanchItemVM>()
{
        new()
        {
            Id = Guid.NewGuid(),
            Type = WorkbanchItemType.Course,
            Title = "My Course"
        },
        new()
        {
            Id = Guid.NewGuid(),
            Type = WorkbanchItemType.Article,
            Title = "My Article"
        }
    };
    }
}
