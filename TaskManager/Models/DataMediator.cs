using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using Document = TaskManager.Data.Document;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Models
{
    public class DataMediator
    {
        public static string ActiveUserName {get; set;}


        public async Task<int> GetUserIdByLoginDataAsync(string loginName, string password)
        {
            int userId = await Task<int>.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    return dbContext.Users.FirstOrDefault(u => u.Login == loginName && u.Password == password)?.Id ?? -1;
                }
            });

            return userId;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            User? user = await Task<User>.Run(() =>
            {
                return GetUserById(userId);
            });

            return user;
        }

        public User GetUserById(int userId)
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                User user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    dbContext.Entry(user)
                        .Reference(u => u.Company)
                        .Load();
                }
                return user;
            }
        }

        public List<User> GetAllSolvers()
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                return dbContext.Users.Where(u => u.Company.IsSolver).ToList();
            }
        }

        public async Task<int> CreateTaskAsync(TaskManager.Data.Task task, int creatorUserId)
        {
            return await Task.Run(() =>
            {

                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    task.CreatorUserId = creatorUserId;
                    dbContext.Add(task);
                    dbContext.SaveChanges();

                    return task.Id;
                }
            });
        }
        public int CreateTask(TaskManager.Data.Task task, int creatorUserId)
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                task.CreatorUserId = creatorUserId;
                dbContext.Add(task);
                dbContext.SaveChanges();

                return task.Id;
            }
        }

        public async Task UpdateTaskDetailsAsync(TaskManager.Data.Task task)
        {
            await Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    dbContext.Entry(task).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            });
        }
        public void UpdateTaskDetails(TaskManager.Data.Task task)
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                dbContext.Entry(task).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public Data.Task GetTaskById(int taskId)
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                return dbContext.Tasks.FirstOrDefault(t => t.Id == taskId);
            }
        }


        public async Task<IQueryable<TaskManager.Data.Task>> GetTasksAsync(int? filterCompanyId = null, int? filterUserId = null)
        {
            return await Task.Run(() => {
                return GetTasks(filterCompanyId, filterUserId);
            });
        }

        public IQueryable<TaskManager.Data.Task> GetTasks(int? filterCompanyId = null, int? filterUserId = null)
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                return dbContext.Tasks.Where(t => (filterCompanyId == null ? true : t.CreatorUser.CompanyId == filterCompanyId) &&
                                                           (filterUserId == null ? true : (t.CreatorUserId == filterUserId || t.SolverUserId == filterUserId)))
                                                      .Include("CreatorUser").Include("SolverUser").Include("CreatorUser.Company").Include("CheckLists").ToArray().AsQueryable();
            }
        }

        public async Task<IQueryable<TaskManager.Data.Document>> GetDocumentsByTaskIdAsync(int taskId)
        {
            IQueryable<TaskManager.Data.Document> retCollection = null;

            await System.Threading.Tasks.Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    var documents = dbContext.Documents.Where(d => d.TaskId == taskId);

                    // Let's make copy to omit big data redundant transfer
                    //
                    List<TaskManager.Data.Document> docsCopy = new();
                    foreach (var docOri in documents)
                    {
                        docsCopy.Add(new Data.Document() { Id = docOri.Id, Title = docOri.Title, DocumentType = docOri.DocumentType });
                    }
                    retCollection = docsCopy.AsQueryable();
                }
            });

            return retCollection;
        }

        public async Task<bool> AddDocumentAsync(Document document, string docFileName)
        {
            return await System.Threading.Tasks.Task<bool>.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    try
                    {
                        document.Content = File.ReadAllBytes(docFileName);
                        dbContext.Add(document);
                        dbContext.SaveChanges(); // SaveChangesAsync() nebylo spolehlivé??
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
            });
        }

        public async Task<(byte[], string)> GetDocumentContentAsync(int documentId)
        {
            byte[] content = null;
            string fileName = null;

            await Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    Document doc = dbContext.Documents.First(d => d.Id == documentId);
                    content = doc.Content;
                    fileName = doc.FileName;
                }
            });

            return (content, fileName);
        }

        public async System.Threading.Tasks.Task DeleteDocumentByIdAsync(int id)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    Document document = dbContext.Documents.First(c => c.Id == id);
                    dbContext.Remove(document);
                    dbContext.SaveChanges();
                }
            });
        }

        public async Task<IQueryable<TaskManager.Data.CheckList>> GetCheckListByTaskIdAsync(int taskId)
        {
            return await Task.Run(() => { 
                return  GetCheckListByTaskId(taskId);
            });
        }

        public IQueryable<TaskManager.Data.CheckList> GetCheckListByTaskId(int taskId)
        {
            IQueryable<TaskManager.Data.CheckList> retCollection = null;

            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                retCollection = dbContext.CheckLists.Where(d => d.TaskId == taskId).ToArray().AsQueryable();
            }

            return retCollection;
        }

        public async Task<TaskManager.Data.CheckList> GetCheckListItemByIdAsync(int id)
        {
            return await Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    return dbContext.CheckLists.FirstOrDefault(c => c.Id == id);
                }
            });
        }

        public async Task<int> AddCheckListItemAsync(CheckList checkListItem)
        {
            return await Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    dbContext.Add(checkListItem);
                    dbContext.SaveChanges();
                }

                return checkListItem.Id;
            });
        }

        public async Task DeleteCheckListItemByIdAsync(int id)
        {
            await Task.Run(() => {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    CheckList checkListItem = dbContext.CheckLists.First(c => c.Id == id);
                    dbContext.Remove(checkListItem);
                    dbContext.SaveChanges();
                }
            });
        }

        public async Task UpdateCheckListItemDetailsAsync(TaskManager.Data.CheckList item)
        {
            await Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    dbContext.Entry(item).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            });
        }

        public List<Chat> GetMessagesForTask(int taskId)
        {
            using (TaskManagerContext dbContext = new TaskManagerContext())
            {
                return dbContext.Chats.Where(c => c.TaskId == taskId).Include("CreatorUser").ToList();
            }
        }

        public async Task AddChatMessageAsync(int taskId, int creatorUserId, string text, DateTime time)
        {
            await Task.Run(() =>
            {
                using (TaskManagerContext dbContext = new TaskManagerContext())
                {
                    Chat newChatMessage = new Chat();
                    newChatMessage.TaskId = taskId;
                    newChatMessage.CreatorUserId = creatorUserId;
                    newChatMessage.Text = text;
                    newChatMessage.DateCreated = time;
                    dbContext.Add(newChatMessage);
                    dbContext.SaveChanges();
                }
            });
        }
    }
}
