# Copilot Instructions

## Tech Stack and Architecture

- **Primary Language:** C#
- **Frameworks:** Xamarin (for cross-platform mobile apps), .NET (Profile111, v4.5), UWP
- **Solution Structure:** Multi-project Visual Studio solution targeting Android, iOS, UWP, and shared logic
- **Key Technologies:** Microsoft Cognitive Services APIs for image, text, and speech analysis

## Build and Test

- **Build:** Open `ExpenseIt/InvoiceIt.sln` in Visual Studio 2015+ (Windows or Mac with Xamarin)
    - Build configurations: `Debug` and `Release` for each platform (Android, iOS, UWP)
    - Use the Visual Studio build/run buttons for each target project
- **Test:** No explicit test projects found; manual testing via app deployment to emulator/device

## Project Structure

- `.github/` — GitHub configuration (no copilot-instructions.md by default)
- `ExpenseIt/`
  - `completed-code/` — Full solution with all code
    - `ExpenseIt.Droid/` — Android project
    - `ExpenseIt.iOS/` — iOS project
    - `ExpenseIt.UWP/` — UWP project
    - `ExpenseIt/` — Shared logic (PCL)
    - `InvoiceIt.sln` — Visual Studio solution file
  - `starter-code/` — Starter template for exercises
- `talk/` — Presentation slides and related materials
- `README.md` — Overview, links to blog posts, and usage terms

## Conventions

- **Cross-platform:** Use shared code in the PCL project; platform-specific code in respective folders
- **NuGet:** Dependencies managed via NuGet in each project file
- **Configuration:** Target framework and build settings defined in each `.csproj`
- **Naming:** Projects and namespaces follow `InvoiceIt`/`ExpenseIt` pattern

## Key Files

- `ExpenseIt/InvoiceIt.sln` — Main entry point for development
- `ExpenseIt/ExpenseIt/InvoiceIt.csproj` — Shared logic project
- `ExpenseIt/ExpenseIt.Droid/InvoiceIt.Droid.csproj` — Android app
- `ExpenseIt/ExpenseIt.iOS/InvoiceIt.iOS.csproj` — iOS app
- `ExpenseIt/ExpenseIt.UWP/InvoiceIt.UWP.csproj` — UWP app
- `README.md` — Project overview and links

## Notes

- Data submitted to sample apps may be retained by Microsoft for service improvement (see README).
- For Cognitive Services API keys, follow instructions in the code or README.
- For more details, see the Xamarin blog posts linked in the README.