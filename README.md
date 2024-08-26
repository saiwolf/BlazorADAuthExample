# Blazor .NET 8 with Identity & AD Integration

This is a Blazor .NET 8 application leverage Microsoft ASP.NET Core Identity with some customizations to
use on-prem Active Directory to do password checking.

Since this project leverages Identity, a DB is created to store user information. Since AD is used for password
checking, the password is **never** stored in the DB. Only user information pertinent to the app is stored.

## TODO (As of 8/26/2024)

- [ ] Finish Admin Role pages
- [x] Finish Admin User Pages
- [x] Add User Claims and Group Info
- [x] Override Identity functions to check on-prem AD for password validation

## Requirements

- On-premises Active Directory (at least one DC should be reachable from the machine the app runs on).
  > You can edit `/Services/Auth/AdUserService.cs` to alter the `PrincipalContext` calls to customize how the app connects to AD. By default, it uses the 'default' domain of the host and binds to the base DN.
- The host must be running Windows. This is due to `System.DirectoryServices` being used for validation.
- .NET 8
- SQL Server (LocalDB was used to design the project). If you want to use another provider, you will have to modify the project accordingly and regenerate the Migrations.

## Considerations

- The app, by default, is configured to require a logged in user to access any resource other than the login page. While the DB seed routine creates a `Users` role, it is not utilized by default.
- The render mode of the app is set to [Static Server Rendering](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0) by default.
- The Nav Menu makes use of `<AuthorizedView>` to control what's shown to anonymous users and logged in users.

## First Time Setup

- Clone this project and change to the app directory.
- run `dotnet restore` to restore the NuGet packages
- Remove the `<UserSecrets>` line in the .csproj file.

### If you're using User Secrets
> And you really should be!
- Regenerate the user secrets via `dotnet user-secrets init`.
  > Visual Studio users can right click on the project and select 'Manage User Secrets'. Otherwise, look in `%APPDATA%\Microsoft\User Secrets` in Windows.
- Copy the contents of `appsettings.Example.json` into the user secrets file and edit accordingly.

### If you're not using User Secrets
- Copy the contents of `appsettings.Example.json` into a new file called `appsettings.Development.json` and edit accordingly.

### Run the DB Seed routine
- Ensure the project builds with `dotnet build`
- Edit `/Data/DbSeed.cs` and customize the users accordingly. **They must be valid AD users**.
- Run the seed routine: `dotnet run -- seed=True`

## Running the app
> It is highly recommended to use Visual Studio, Rider, or VS Code for debugging of the app.
- The app can be run from the command line with `dotnet run` in the top project directory.

## What If I Want Interactivity Globally?

One major caveat of Interactive Render Modes in Blazor is the lack
of access to `HttpContext`. Server mode runs over a SignalR circuit, and WASM runs
in the client. Because of this, Identity uses Static Server Rendering with its razor
components and code.

If you want to enable Interactivity on a per-component basis, then no changes are
required. Just make sure to not add any `@rendermode` to any Identity pages under `/Component/Accounts/Pages`.

However, if you want to enable Interactivity globally, then some changes are required:

- Uncomment the `@code {}` block in `/Components/App.razor`
- Add `@rendermode="RenderModeForPage"` to both `<HeadOutlet />` and `<Routes />`

> **Note**: Any page starting with `/account` will be rendered as SSR with this method, as these
pages rely on access to `HttpContext` which is not present otherwise.