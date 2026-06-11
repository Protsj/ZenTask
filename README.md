# ZenTask — Advanced Personal Productivity & Task Management System

ZenTask is a modern, high-performance desktop productivity application designed to streamline daily time management, goal tracking, and focus refinement. Built entirely on **C# and .NET**, the platform leverages a robust **WPF (Windows Presentation Foundation)** architecture separated cleanly from an independent business logic layer. 

The application is structured as a complete enterprise-grade Visual Studio Solution, seamlessly mapping complex Object-Oriented Programming (OOP) paradigms, SOLID software design principles, multi-threaded operations, asynchronous processing, and meta-programming to deliver an efficient user experience.

---

## 🚀 Key Features

* **🔁 Habit Tasks & Automated Audits:** Built-in habit loop tracking with live streak count indicators (`🔥 X days streak`). Implements an automatic calendar audit loop upon launch that evaluates completion dates (`LastCompletedDate`). If a new day arrives, checkboxes are cleared automatically while protecting the streak; if a day is skipped, the streak is dynamically reset using encapsulation contracts (`ResetCycle()`).
* **⏱️ Focus Tasks (Pomodoro Engine):** Integrated focus enhancement sessions. Includes a dedicated, borderless **Pomodoro Session Window** with an intelligent, adaptive digital timer countdown. It seamlessly switches display formats based on the duration size (`H:MM:SS` for long focus blocks and `MM:SS` for short blocks), updating state fields thread-safely via a managed GUI dispatcher clock.
* **🚨 Urgent Tasks (Real-Time Deadlines):** Task nodes bound to tight calendar timeframes. Features a real-time tracking badge reflecting hours and minutes remaining until expiration (`⏳ Xh Ym left`). Driven by a central continuous GUI updater timer, it dynamically switches into an aggressive warning state (`🚨 Overdue`) the exact minute a target threshold passes.
* **📋 List Tasks (Cascading Checklists):** Specialized polymorphic checklist workflows. Features fully interactive multi-tier sub-task items. Checking off all nested children automatically marks the parent task group as completed; conversely, unchecking the parent cascade-clears the inner states across the persistence layer.

---

## 🏗️ Solution Architecture

The project is modularly isolated into distinct logical sub-layers within a single solution structure to maintain strict boundary concerns:

```
ZenTask/
│
├── ZenTask.Core/           # Core Domain Models, Services, and Business Logic
├── ZenTask.WPF/            # Windows Presentation UI Layer (MVVM-aligned viewports)
├── ZenTask.Console/        # Legacy CLI Interface / Diagnostics and Data Inspection
└── ZenTask.Tests/          # Automated Unit Testing Environment (Invariants validation)
```

1.  **`ZenTask.Core` (Domain & Services):** Contains the primary abstract entities (`BaseTask`), distinct entity specifications (`HabitTask`, `UrgentTask`, `MeetingTask`, `FocusTask`, `CallTask`, `ListTask`), completion contracts (`ICompletable`), and the orchestrating core manager (`TaskManager`).
2.  **`ZenTask.WPF` (Presentation Layer):** Powering the desktop UI. Features an innovative declarative engine (**`UIBuilder`**) that loads visual structures from structured metadata (`.json`), parsing components dynamically to generate layouts at runtime without hardcoded design binds. It also boasts a zero-scrollbar hidden design aesthetic supporting fluid mouse-wheel kinetic scrolls.
3.  **`ZenTask.Console` (CLI Adapter):** A lightweight command-line portal utilizing the same `Core` engines, providing a modular backup for headless environments or low-level diagnostic executions.
4.  **`ZenTask.Tests` (Quality Assurance):** A standalone unit test project ensuring business invariants, data mutation constraints, and execution methods behave predictably under variable state inputs.

---

## 🛠️ Technological Paradigm & Engineering Patterns

The codebase serves as a pristine demonstration of enterprise software design patterns:

| Pillar / Principle | Technical Application in ZenTask |
| :--- | :--- |
| **S** (Single Responsibility) | Deep structural separation: `UIBuilder` handles visual parsing, `SqliteTaskStorage` handles data access layers (DAL), and `TaskManager` commands application state. |
| **O** (Open/Closed Principle) | Fully extensible domain architecture. Adding new productivity modules (e.g., `ProjectTask`) requires zero modifications to existing abstract pipelines. |
| **L** (Liskov Substitution) | Complete runtime polymorphism. The storage layer, filtering delegates, and card renderer seamlessly manage unified lists of polymorphic entities via `BaseTask` references. |
| **I** (Interface Segregation) | Custom decoupled contracts. Optional markers like `ICompletable` keep fixed tasks (like `MeetingTask`) unpolluted by invalid boolean flag properties. |
| **D** (Dependency Inversion) | Strict loose coupling. View components depend exclusively on abstracted service pipelines, making the core backend completely independent of the front-end graphical ecosystem. |
| **Advanced C# (Reflection)** | Leveraged in the `EditTaskWindow` to inspect runtime object metadata. It tracks and clears internal compiler-generated auto-backing collections (`<Items>k__BackingField`) dynamically without exposing internal lists or violating encapsulation rules. |
| **Asynchronous & Threading** | Database interactions are fully asynchronous (`SaveTaskAsync`, `LoadTasksAsync`). Heavy disk I/O routines run off the main rendering thread to prevent desktop UI freezing. Visual tickers safely interact with view bounds using the `DispatcherTimer` synchronization context. |
| **Data Persistence** | Powered by a localized, compact **SQLite** relational engine managing tables with relational entity schemas, ensuring quick recovery across software boot cycles. |

---

## ⚙️ Setup & Installation

### Prerequisites
* Windows 10 / 11 Operating System
* .NET 8.0 SDK or higher
* Visual Studio 2022 (with *.NET Desktop Development* workload installed)

### Build Instructions
1.  Clone this repository to your local machine:
    ```bash
    git clone https://github.com/yourusername/ZenTask.git
    ```
2.  Navigate to the solution directory and restore NuGet dependencies:
    ```bash
    dotnet restore ZenTask.sln
    ```
3.  Compile the entire solution package:
    ```bash
    dotnet build ZenTask.sln --configuration Release
    ```
4.  Execute the WPF Desktop client workspace:
    ```bash
    dotnet run --project ZenTask.WPF/ZenTask.WPF.csproj
    ```

---