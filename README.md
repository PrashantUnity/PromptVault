# PromptVault 🗃️

A modern, responsive Blazor WebAssembly application
## ✨ Features

### 🎯 Core Functionality
- **Prompt Management**: Create, edit, delete, and organize AI prompts
- **Categorization**: Organize prompts into categories (Marketing, Development, Creative Writing, Business, etc.)
- **Search & Filter**: Powerful search across titles, content, descriptions, and tags
- **Favorites System**: Mark prompts as favorites for quick access
- **Rating System**: Rate prompts and view average ratings
- **Usage History**: Track recently used prompts
- **Export/Import**: Backup and restore your prompt library

### 🎨 User Experience
- **Responsive Design**: Optimized for desktop, tablet, and mobile devices
- **Dark/Light Theme**: Toggle between themes with persistent preferences
- **Modern UI**: Clean, intuitive interface built with Tailwind CSS
- **Loading Animations**: Beautiful loading screens and transitions
- **Real-time Updates**: Instant UI updates with reactive state management

### 🔧 Technical Features
- **Local Storage**: All data persisted locally in browser storage
- **External Data Source**: Automatically loads prompts from external repository
- **Placeholder Support**: Dynamic placeholder parsing for customizable prompts
- **PWA Ready**: Progressive Web App capabilities
- **Blazor WebAssembly**: Client-side rendering for fast performance

## 🚀 Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- Modern web browser with WebAssembly support

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/promptvault.git
   cd promptvault
   ```

2. **Navigate to the project directory**
   ```bash
   cd PromptsValut
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open your browser**
   Navigate to `https://localhost:7080` or `http://localhost:5001`

### Development

For development with hot reload:
```bash
dotnet watch run
```

## 🏗️ Project Structure

```
PromptsValut/
├── Components/           # Reusable UI components
│   ├── Modals/          # Modal dialogs (Create, Export/Import, etc.)
│   └── UI/              # UI components (Cards, Filters, etc.)
├── Constants/           # Application constants (SVG icons)
├── Layout/             # Layout components
├── Models/             # Data models (Prompt, Category, etc.)
├── Pages/              # Application pages
├── Services/           # Business logic services
├── wwwroot/            # Static web assets
│   ├── css/           # Stylesheets
│   ├── js/            # JavaScript files
│   └── index.html     # Main HTML file
└── Program.cs          # Application entry point
```

## 📱 Usage

### Creating Prompts
1. Click the **Create** button
2. Fill in the prompt details:
   - Title and description
   - Category selection
   - Tags for better organization
   - Difficulty level
   - Usage notes and estimated time
3. Save your prompt

### Managing Prompts
- **Search**: Use the search bar to find prompts by title, content, or tags
- **Filter**: Select categories to filter prompts
- **Favorites**: Click the heart icon to add prompts to favorites
- **Rate**: Use the star rating system to rate prompts
- **View Details**: Click on any prompt card to view full details

### Data Management
- **Export**: Download your prompt library as JSON
- **Import**: Upload previously exported data
- **History**: View recently used prompts
- **Favorites**: Access your favorite prompts quickly

## 🎨 Customization

### Themes
The application supports both light and dark themes. The theme preference is automatically saved and restored.

### Categories
Default categories include:
- Marketing
- Development
- Creative Writing
- Business
- Education
- Technology
- Fun
- Productivity
- Data Analysis
- Testing
- General

### External Data
The application automatically loads prompts from an external data source. You can refresh this data using the refresh functionality.

## 🔧 Configuration

### External Data Source
The application loads prompts from: `https://raw.githubusercontent.com/codefrydev/Data/refs/heads/main/Prompt/data.json`

### Local Storage Keys
- `promptvault-state`: Main application state
- Theme preferences and other user settings

## 🛠️ Technologies Used

- **Blazor WebAssembly** (.NET 10.0)
- **Tailwind CSS** for styling
- **JavaScript** for client-side functionality
- **System.Text.Json** for data serialization
- **HttpClient** for external data fetching

## 📦 Dependencies

- `Microsoft.AspNetCore.Components.WebAssembly` (10.0.0-rc.1.25451.107)
- `Microsoft.AspNetCore.Components.WebAssembly.DevServer` (10.0.0-rc.1.25451.107)
- `Microsoft.Extensions.Http` (10.0.0-rc.1.25451.107)
- `Microsoft.JSInterop` (10.0.0-rc.1.25451.107)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- External prompt data provided by [codefrydev/Data](https://github.com/codefrydev/Data)
- Icons and UI inspiration from various design systems
- Built with love using Blazor and modern web technologies

## 📞 Support

If you encounter any issues or have questions:
1. Check the [Issues](https://github.com/yourusername/promptvault/issues) page
2. Create a new issue with detailed information
3. Include browser console logs if applicable

---

**PromptVault** - Your AI prompt library, organized and beautiful. 🚀
