using Blazored.LocalStorage;
using Client.Models;
using Client.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class CourseService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;

        public CourseService(ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }

        public async Task<CourseFullInfoVM> GetCourse(Guid id)
        {
            //var course = await _httpClient.GetFromJsonAsync<CourseFullInfoVM>($"https://localhost:7001/api/courses/?id={id}");
            //Console.WriteLine(course.Id);
            //return course;

            //if (devMode)
            //{
            //    //if (await _localStorageService.ContainKeyAsync(devKey + id))
            //    //{
            //    //    return await _localStorageService.GetItemAsync<CourseFullInfoViewModel>(devKey + id);
            //    //}
            //    //else
            //    //{
            //    if (id == 1)
            //    {
            //        var course = GetExample();
            //        await SaveCourseToLocalStorage(course);
            //        return course;
            //    }
            //    //}
            //}
            if (await _localStorageService.ContainKeyAsync("workbanch_" + id))
            {
                var course = await _localStorageService.GetItemAsync<CourseFullInfoVM>("workbanch_" + id);
                course.Rating = new RatingModel { Rating = 0, VotersCount = 0 };
                return course;
            }
            if (id == new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }))
                return GetExample(id);
            return null;
        }

        public async Task CreateCourse(CreateCourseCommand course)
        {
            await _httpClient.PostAsJsonAsync("https://localhost:7001/api/courses", course);
        }

        public async Task EditCourse(CourseFullInfoVM course)
        {
            var newCourse = new CreateCourseCommand
            {
                Title = course.Title,
                ImgSrc = course.ImgSrc,
                Description = course.Description,
                Difficulty = course.Difficulty
            };
            await _httpClient.PostAsJsonAsync("https://localhost:7001/api/courses", newCourse);
        }

        //public async Task SaveCourseToLocalStorage(CourseFullInfoViewModel course)
        //{
        //    await _localStorageService.SetItemAsync(devKey + course.Id, course);
        //}

        public async Task<List<CourseMainInfoVM>> GetAllCourses(string searchString, CourseSearchType type)
        {
            //return await _httpClient.GetFromJsonAsync<List<CourseMainInfoVM>>($"https://localhost:7001/api/courses/all/?searchString={searchString}");

            var allItems = await _localStorageService.GetItemAsync<List<WorkbanchItemVM>>("workbanch_all");
            var allCourses = GetCoursesMock();
            if (allItems?.Count > 0)
            {
                foreach (var item in allItems)
                {
                    if (item.Type == WorkbanchItemType.Course)
                    {
                        var course = await _localStorageService.GetItemAsync<CourseMainInfoVM>("workbanch_" + item.Id);
                        allCourses = allCourses.Prepend(course).ToList();
                    }
                }
            }
            return allCourses.Where(c => c.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private List<CourseMainInfoVM> GetCoursesMock() => new List<CourseMainInfoVM>
        {
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }),
                Title = "Основы C#",
                ImgSrc = "https://avatars.mds.yandex.net/get-zen_doc/4423710/pub_608fda25a38d215d4e183cba_60928147de56f009bc84b924/scale_1200"
            },
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Основы JavaScript",
                ImgSrc = "https://www.creativefabrica.com/wp-content/uploads/2019/02/Monogram-JS-Logo-Design-by-Greenlines-Studios.jpg"
            },
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Архитектурные паттерны",
                ImgSrc = "https://phonoteka.org/uploads/posts/2021-04/1619236371_2-phonoteka_org-p-chertezhi-fon-2.jpg"

            },
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Микросервисы в .NET",
                ImgSrc = "https://i.pinimg.com/originals/db/72/fd/db72fdb8d549c3eccbf52a45e7f3114f.jpg"
            },
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Angular",
                ImgSrc = "https://blog.knoldus.com/wp-content/uploads/2020/06/angular.png"
            },
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Blazor",
                ImgSrc = "https://www.atsistemas.com/dam/jcr:f50d73f5-76b1-42d4-97e9-c1f33030eec1/Microsoft-Blazor-1375x760.jpg"
            },
            new CourseMainInfoVM
            {
                Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Advanced C#",
                ImgSrc = "https://i.ytimg.com/vi/s8ru33IIQzc/maxresdefault.jpg"
            },
        };

        private CourseFullInfoVM GetExample(Guid id)
        {
            var root = new RecordVM
            {
                Id = new Guid(new byte[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Type = RecordType.Root,
                Title = "ROOT"
            };
            var chapter1 = new RecordVM
            {
                Id = new Guid(new byte[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Type = RecordType.Group,
                Title = "Введение в C#"
            };
            var article1_1 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Язык C# и платформа .NET"
            };
            var article1_2 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Начало работы с Visual Studio. Первая программа"
            };
            var article1_3 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Компиляция в командной строке"
            };
            var chapter2 = new RecordVM
            {
                Id = new Guid(new byte[] { 1, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Type = RecordType.Group,
                Title = "Основы программирования на C#"
            };
            var article2_1 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Структура программы"
            };
            var subchapter2_2 = new RecordVM
            {
                Type = RecordType.Group,
                Id = new Guid(new byte[] { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Работа с данными"
            };
            var article2_2_1 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Переменные"
            };
            var article2_2_2 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Литералы"
            };
            var article2_2_3 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Типы данных"
            };
            var article2_3 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Консольный ввод-вывод"
            };
            var subchapter2_4 = new RecordVM
            {
                Type = RecordType.Group,
                Id = new Guid(new byte[] { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Операции с данными"
            };
            var article2_4_1 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Арифметические операции"
            };
            var article2_4_2 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Поразрядные операции"
            };
            var article2_4_3 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Операции присваивания"
            };
            var article2_4_4 = new RecordVM
            {
                Type = RecordType.Article,
                Id = new Guid(new byte[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Преобразование базовых типов данных"
            };
            root.Children = new() { chapter1, chapter2 };
            chapter1.Children = new() { article1_1, article1_2, article1_3 };
            chapter2.Children = new() { article2_1, subchapter2_2, article2_3, subchapter2_4 };
            subchapter2_2.Children = new() { article2_2_1, article2_2_2, article2_2_3 };
            subchapter2_4.Children = new() { article2_4_1, article2_4_2, article2_4_3, article2_4_4 };

            var course = new CourseFullInfoVM()
            {
                Id = id,
                Title = "Основы C#",
                Description = "Курс рассчитан на людей с минимальным опытом программирования и знакомит с основами синтаксиса C#" +
                        " и стандартными классами .NET, с основами ООП и базовыми алгоритмами.",
                //ImgSrc = "https://avatars.mds.yandex.net/get-zen_doc/4423710/pub_608fda25a38d215d4e183cba_60928147de56f009bc84b924/scale_1200",
                Difficulty = Difficulty.Beginer,
                Rating = new RatingModel
                {
                    Rating = 4.5,
                    VotersCount = 108
                },
                RootRecord = root
            };
            return course;
        }

        public async Task<List<CourseCommitMainInfoVM>> GetHistory(Guid id)
        {
            return new()
            {
                new()
                {
                    Id = new Guid(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                    CreatedAt = new DateTime(2021, 05, 29, 14, 10, 34),
                    CreatedBy = "CourseOwner",
                    CommitState = CommitState.Commited,
                    Title = "Created",
                    Description = "Course created",
                    PrevCommitId = null,
                    Type = CourseCommitType.Added
                },
                //new()
                //{
                //    Id = Guid.NewGuid(),
                //    CreatedAt = new DateTime(2021, 05, 15, 21, 31, 47),
                //    CreatedBy = "foxxy",
                //    CommitState = CommitState.Commited,
                //    Title = "Added new article",
                //    Description = "Added new article \"Консольный ввод-вывод\"",
                //    PrevCommitId = new Guid(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                //    Type = CourseCommitType.Added
                //}
            };
        }
    }
}
