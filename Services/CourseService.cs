using Blazored.LocalStorage;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Services
{
    public class CourseService
    {
        private const string devKey = "course_dev_";

        private readonly ILocalStorageService _localStorageService;

        public CourseService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<CourseFullInfoViewModel> GetCourse(int id, bool devMode)
        {
            if (devMode)
            {
                if (await _localStorageService.ContainKeyAsync(devKey + id))
                {
                    return await _localStorageService.GetItemAsync<CourseFullInfoViewModel>(devKey + id);
                }
                else
                {
                    if (id == 1)
                    {
                        var course = GetExample();
                        await SaveCourseToLocalStorage(course);
                        return course;
                    }
                }
            }
            if (id == 1)
                return GetExample();
            return null;
        }

        public async Task SaveCourseToLocalStorage(CourseFullInfoViewModel course)
        {
            await _localStorageService.SetItemAsync(devKey + course.Id, course);
        }

        private CourseFullInfoViewModel GetExample()
        {
            var root = new RecordMainInfoViewModel
            {
                Id = 0,
                Type = RecordType.Root,
                Name = "ROOT"
            };
            var chapter1 = new RecordMainInfoViewModel
            {
                Id = 1,
                Type = RecordType.Group,
                Name = "Глава 1"
            };
            var subchapter1_1 = new RecordMainInfoViewModel
            {
                Id = 2,
                Type = RecordType.Group,
                Name = "Подглава 1"
            };
            var article1_1_1 = new RecordMainInfoViewModel
            {
                Id = 3,
                Type = RecordType.Article,
                TargetId = 1,
                Name = "Статья 1"
            };
            var article1_1_2 = new RecordMainInfoViewModel
            {
                Id = 4,
                Type = RecordType.Article,
                TargetId = 2,
                Name = "Статья 2"
            };
            var subchapter1_2 = new RecordMainInfoViewModel
            {
                Id = 5,
                Type = RecordType.Group,
                Name = "Подглава 2"
            };
            var article1_2_1 = new RecordMainInfoViewModel
            {
                Id = 6,
                Type = RecordType.Article,
                TargetId = 3,
                Name = "Статья 1"
            };
            var article1_2_2 = new RecordMainInfoViewModel
            {
                Id = 7,
                Type = RecordType.Article,
                TargetId = 4,
                Name = "Статья 2"
            };
            root.Children = new() { chapter1 };
            chapter1.Children = new() { subchapter1_1, subchapter1_2 };
            subchapter1_1.Children = new() { article1_1_1, article1_1_2 };
            subchapter1_2.Children = new() { article1_2_1, article1_2_2 };

            var course = new CourseFullInfoViewModel()
            {
                Id = 1,
                Name = "Основы C#",
                Description = "Курс разработан для студентов первого года обучения компьютерных специальностей УрФУ." +
                        " Рассчитан на людей с минимальным опытом программирования и знакомит с основами синтаксиса C#" +
                        " и стандартными классами .NET, с основами ООП и базовыми алгоритмами.",
                ImgSrc = "https://www.secretorange.co.uk/media/TagImage/c-sharp.png",
                Difficulty = Difficulty.Intermediate,
                Records = new() { root, subchapter1_1, article1_2_1, chapter1, subchapter1_2, article1_1_1, article1_1_2, article1_2_2 }
            };
            return course;
        }
    }
}
