
// Adds initial migration to the project
dotnet ef migrations add InitialCreate --project FunBooksAndVideos.Infrastructure --startup-project FunBooksAndVideos.API

// Apply migrations sequentially to the database
dotnet ef database update --project FunBooksAndVideos.Infrastructure --startup-project FunBooksAndVideos.API

// Delete the latest migration 
dotnet ef migrations remove --project FunBooksAndVideos.Infrastructure --startup-project FunBooksAndVideos.API
