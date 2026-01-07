namespace TodoApi.DTO.Request
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}