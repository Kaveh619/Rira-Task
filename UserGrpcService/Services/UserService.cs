using Grpc.Core;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace UserGrpcService.Services
{
    public class UserService :UserGrpcService.UserService.UserServiceBase
    {
        private static readonly ConcurrentDictionary<string, User> Users = new();

        public override Task<UserId> CreateUser(User request, ServerCallContext context)
        {
            var userId = request.Id;
            if (Users.ContainsKey(userId))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists"));
            }

            Users[userId] = request;
            return Task.FromResult(new UserId { Id = userId });
        }

        public override Task<User> GetUser(UserId request, ServerCallContext context)
        {
            if (!Users.TryGetValue(request.Id, out var user))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return Task.FromResult(user);
        }

        public override Task<Empty> UpdateUser(User request, ServerCallContext context)
        {
            if (!Users.ContainsKey(request.Id))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            Users[request.Id] = request;
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> DeleteUser(UserId request, ServerCallContext context)
        {
            if (!Users.TryRemove(request.Id, out _))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            return Task.FromResult(new Empty());
        }

        public override Task<UserList> ListUsers(Empty request, ServerCallContext context)
        {
            var userList = new UserList();
            userList.Users.AddRange(Users.Values);
            return Task.FromResult(userList);
        }
    }
}
