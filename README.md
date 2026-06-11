# 📅 ZenTask: Smart Daily Planner

**ZenTask** is a modern desktop application with a graphical user interface (WPF) for time management and personal productivity. The program combines the flexibility of managing diverse tasks in a single optimized core, allowing the user to track daily habits, plan deep focus sessions, organize meetings, and much more.

Developed as part of a coursework project in the "Object-Oriented Programming" discipline.

## 🚀 Key Features

* **Flexible task system (6 types):**
  * `UrgentTask`: High-priority urgent tasks.
  * `MeetingTask`: Meetings with specified time and participants.
  * `HabitTask`: Habit tracker for daily execution.
  * `FocusTask`: Deep focus tasks (with a Pomodoro timer) and duration tracking.
  * `ListTask`: Container task with a checklist. Automatically marked as completed only when all internal subtasks are checked.
  * `CallTask`: Calls with contact details, platform, and reminders.
* **Data-Driven UI:** Interface configurations (formats, window and button sizes) are stored and managed via JSON templates.
* **Modern UI Validation:** "Soft" input field validation (e.g., highlighting errors without annoying standard popup windows) and the use of custom dialog windows (`CustomMessageBox`).
* **Reliable Data Storage:** Local SQLite database with automatic structure generation (Code-First).

## 🛠 Technology Stack

* **Language:** C# (.NET 8)
* **UI Framework:** WPF (Windows Presentation Foundation)
* **Database:** SQLite
* **ORM:** Entity Framework Core (EF Core)
* **Serialization:** `System.Text.Json` with polymorphism support (`[JsonDerivedType]`)
* **Testing:** xUnit (Unit tests and in-memory integration tests)

## 🏗 Architecture and Patterns

The project is built on an N-layer architecture, where the visual part (`ZenTask.WPF`) is clearly separated from the business logic (`ZenTask.Core`).

* **SOLID principles:** Strict adherence to single responsibility and dependency inversion.
* **Design Patterns:** MVVM, Observer (Events, Async), Dependency Inversion, Factory (`UIBuilder`).
* **Table-Per-Type (TPT) DB Strategy:** To store the task class hierarchy (`BaseTask` and 6 descendants), the TPT approach is used, ensuring perfect data normalization, referential integrity support, and allowing for fast polymorphic queries via Entity Framework Core.

## ⚙️ How to Run Locally

1. Clone the repository to your local machine.
2. Open the `ZenTask.sln` solution file in Visual Studio 2022.
3. In the Solution Explorer, right-click on the **`ZenTask.WPF`** project and select **Set as StartUp Project**.
4. Press the **Start** button (or `F5`).
   * *Note:* Upon the first launch, the program will automatically (Code-First) generate the `Tasks.db` database file and the necessary JSON interface templates.

## 🧪 Testing

The project is covered by unit and integration tests using `xUnit`.
To run tests, go to the top menu in Visual Studio:
**Test -> Run All Tests** (or use the shortcut `Ctrl + R, A`).