namespace PromptsValut.Constants;

/// <summary>
/// Contains all SVG icon strings used throughout the application.
/// 
/// Usage Examples:
/// 1. Direct usage with default classes:
///    @((MarkupString)SvgIcons.Heart)
/// 
/// 2. Usage with custom classes:
///    @((MarkupString)SvgIcons.GetIcon("heart", "h-6 w-6 text-red-500"))
/// 
/// 3. In Razor components, add this using statement:
///    @using PromptsValut.Constants
/// 
/// Available Icons:
/// - Navigation: menu, close, search, help
/// - Theme: moon, sun
/// - Actions: heart, history, plus, download, copy, eye, star, clock
/// - Status: check, alert
/// - Files: file, upload
/// - Layout: grid
/// - Calendar: calendar
/// </summary>
public static class SvgIcons
{
    // Navigation Icons
    public const string Menu = """
                               <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <line x1="3" y1="6" x2="21" y2="6"></line>
                                       <line x1="3" y1="12" x2="21" y2="12"></line>
                                       <line x1="3" y1="18" x2="21" y2="18"></line>
                                   </svg>
                               """;

    public const string Close = """
                                <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <line x1="18" y1="6" x2="6" y2="18"></line>
                                        <line x1="6" y1="6" x2="18" y2="18"></line>
                                    </svg>
                                """;

    public const string Search = """
                                 <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                         <circle cx="11" cy="11" r="8"></circle>
                                         <path d="m21 21-4.35-4.35"></path>
                                     </svg>
                                 """;

    public const string Help = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <circle cx="12" cy="12" r="10"></circle>
                                       <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                                       <line x1="12" y1="17" x2="12.01" y2="17"></line>
                                   </svg>
                               """;

    // Theme Icons
    public const string Moon = """
                               <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
                                   </svg>
                               """;

    public const string Sun = """
                              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                      <circle cx="12" cy="12" r="5"></circle>
                                      <line x1="12" y1="1" x2="12" y2="3"></line>
                                      <line x1="12" y1="21" x2="12" y2="23"></line>
                                      <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
                                      <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
                                      <line x1="1" y1="12" x2="3" y2="12"></line>
                                      <line x1="21" y1="12" x2="23" y2="12"></line>
                                      <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
                                      <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
                                  </svg>
                              """;

    // Action Icons
    public const string Heart = """
                                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.29 1.51 4.04 3 5.5l7 7Z"></path>
                                    </svg>
                                """;

    public const string History = """
                                  <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                          <path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8"></path>
                                          <path d="M3 3v5h5"></path>
                                          <path d="M12 7v5l4 2"></path>
                                      </svg>
                                  """;

    public const string Plus = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <line x1="12" y1="5" x2="12" y2="19"></line>
                                       <line x1="5" y1="12" x2="19" y2="12"></line>
                                   </svg>
                               """;

    public const string Download = """
                                   <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                           <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                                           <polyline points="7,10 12,15 17,10"></polyline>
                                           <line x1="12" y1="15" x2="12" y2="3"></line>
                                       </svg>
                                   """;

    // Utility Icons
    public const string Copy = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <rect width="14" height="14" x="8" y="8" rx="2" ry="2"></rect>
                                       <path d="M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2"></path>
                                   </svg>
                               """;

    public const string Eye = """
                              <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                      <path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path>
                                      <circle cx="12" cy="12" r="3"></circle>
                                  </svg>
                              """;

    public const string Star = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <polygon points="12,2 15.09,8.26 22,9.27 17,14.14 18.18,21.02 12,17.77 5.82,21.02 7,14.14 2,9.27 8.91,8.26"></polygon>
                                   </svg>
                               """;

    public const string Clock = """
                                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <circle cx="12" cy="12" r="10"></circle>
                                        <polyline points="12,6 12,12 16,14"></polyline>
                                    </svg>
                                """;

    // Status Icons
    public const string Check = """
                                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <polyline points="20,6 9,17 4,12"></polyline>
                                    </svg>
                                """;

    public const string Alert = """
                                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"></path>
                                        <line x1="12" y1="9" x2="12" y2="13"></line>
                                        <line x1="12" y1="17" x2="12.01" y2="17"></line>
                                    </svg>
                                """;

    // File Icons
    public const string File = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <path d="M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z"></path>
                                       <polyline points="14,2 14,8 20,8"></polyline>
                                   </svg>
                               """;

    public const string Upload = """
                                 <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                         <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                                         <polyline points="17,8 12,3 7,8"></polyline>
                                         <line x1="12" y1="3" x2="12" y2="15"></line>
                                     </svg>
                                 """;

    // Grid and Layout Icons
    public const string Grid = """
                               <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <rect width="7" height="7" x="3" y="3" rx="1"></rect>
                                       <rect width="7" height="7" x="14" y="3" rx="1"></rect>
                                       <rect width="7" height="7" x="14" y="14" rx="1"></rect>
                                       <rect width="7" height="7" x="3" y="14" rx="1"></rect>
                                   </svg>
                               """;

    // Calendar and Date Icons
    public const string Calendar = """
                                   <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                           <rect width="18" height="18" x="3" y="4" rx="2" ry="2"></rect>
                                           <line x1="16" y1="2" x2="16" y2="6"></line>
                                           <line x1="8" y1="2" x2="8" y2="6"></line>
                                           <line x1="3" y1="10" x2="21" y2="10"></line>
                                       </svg>
                                   """;

    // Additional Common Icons
    public const string Trash = """
                                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path d="M3 6h18"></path>
                                        <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"></path>
                                        <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"></path>
                                    </svg>
                                """;

    public const string Play = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <polygon points="5,3 19,12 5,21"></polygon>
                                   </svg>
                               """;

    public const string Code = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <polyline points="16,18 22,12 16,6"></polyline>
                                       <polyline points="8,6 2,12 8,18"></polyline>
                                   </svg>
                               """;

    public const string Lightbulb = """
                                    <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path d="M9 21c0 .55.45 1 1 1h4c.55 0 1-.45 1-1v-1H9v1z"></path>
                                            <path d="M12 2C8.14 2 5 5.14 5 9c0 2.38 1.19 4.47 3 5.74V17c0 .55.45 1 1 1h6c.55 0 1-.45 1-1v-2.26c1.81-1.27 3-3.36 3-5.74 0-3.86-3.14-7-7-7z"></path>
                                        </svg>
                                    """;

    public const string Refresh = """
                                  <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                          <path d="M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8"></path>
                                          <path d="M21 3v5h-5"></path>
                                          <path d="M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16"></path>
                                          <path d="M3 21v-5h5"></path>
                                      </svg>
                                  """;

    // Additional Category Icons
    public const string LayoutGrid = """
                                     <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                             <rect width="7" height="7" x="3" y="3" rx="1"></rect>
                                             <rect width="7" height="7" x="14" y="3" rx="1"></rect>
                                             <rect width="7" height="7" x="14" y="14" rx="1"></rect>
                                             <rect width="7" height="7" x="3" y="14" rx="1"></rect>
                                         </svg>
                                     """;

    public const string TrendingUp = """
                                     <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                             <polyline points="22,7 13.5,15.5 8.5,10.5 2,17"></polyline>
                                             <polyline points="16,7 22,7 22,13"></polyline>
                                         </svg>
                                     """;

    public const string PenTool = """
                                  <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                          <path d="M12 19l7-7 3 3-7 7-3-3z"></path>
                                          <path d="M18 13l-1.5-7.5L2 2l3.5 14.5L13 18l5-5z"></path>
                                          <path d="M2 2l7.586 7.586"></path>
                                          <circle cx="11" cy="11" r="2"></circle>
                                      </svg>
                                  """;

    public const string GraduationCap = """
                                        <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path d="M22 10v6M2 10l10-5 10 5-10 5z"></path>
                                                <path d="M6 12v5c3 3 9 3 12 0v-5"></path>
                                            </svg>
                                        """;

    public const string BarChart3 = """
                                    <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path d="M3 3v18h18"></path>
                                            <path d="M18 17V9"></path>
                                            <path d="M13 17V5"></path>
                                            <path d="M8 17v-3"></path>
                                        </svg>
                                    """;

    public const string PlayCircle = """
                                     <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                             <circle cx="12" cy="12" r="10"></circle>
                                             <polygon points="10,8 16,12 10,16"></polygon>
                                         </svg>
                                     """;

    public const string Target = """
                                 <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                         <circle cx="12" cy="12" r="10"></circle>
                                         <circle cx="12" cy="12" r="6"></circle>
                                         <circle cx="12" cy="12" r="2"></circle>
                                     </svg>
                                 """;

    public const string Zap = """
                              <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                      <polygon points="13,2 3,14 12,14 11,22 21,10 12,10"></polygon>
                                  </svg>
                              """;

    public const string X = """
                            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <line x1="18" y1="6" x2="6" y2="18"></line>
                                    <line x1="6" y1="6" x2="18" y2="18"></line>
                                </svg>
                            """;

    // Additional icons found in other files
    public const string FileText = """
                                   <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                           <path d="M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z"></path>
                                           <polyline points="14,2 14,8 20,8"></polyline>
                                           <line x1="16" y1="13" x2="8" y2="13"></line>
                                           <line x1="16" y1="17" x2="8" y2="17"></line>
                                           <polyline points="10,9 9,9 8,9"></polyline>
                                       </svg>
                                   """;

    public const string Settings = """
                                   <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                           <circle cx="12" cy="12" r="3"></circle>
                                           <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1 1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"></path>
                                       </svg>
                                   """;

    public const string Edit = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
                                       <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
                                   </svg>
                               """;

    public const string CheckCircle = """
                                      <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
                                              <polyline points="22,4 12,14.01 9,11.01"></polyline>
                                          </svg>
                                      """;

    public const string RefreshCw = """
                                    <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <polyline points="23,4 23,10 17,10"></polyline>
                                            <polyline points="1,20 1,14 7,14"></polyline>
                                            <path d="M20.49 9A9 9 0 0 0 5.64 5.64L1 10m22 4l-4.64 4.36A9 9 0 0 1 3.51 15"></path>
                                        </svg>
                                    """;

    public const string Save = """
                               <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                       <path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"></path>
                                       <polyline points="17,21 17,13 7,13 7,21"></polyline>
                                       <polyline points="7,3 7,8 15,8"></polyline>
                                   </svg>
                               """;

    public const string HelpCircle = """
                                     <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                             <circle cx="12" cy="12" r="10"></circle>
                                             <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                                             <line x1="12" y1="17" x2="12.01" y2="17"></line>
                                         </svg>
                                     """;

    public const string Grid3x3 = """
                                  <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                          <rect width="7" height="7" x="3" y="3" rx="1"></rect>
                                          <rect width="7" height="7" x="14" y="3" rx="1"></rect>
                                          <rect width="7" height="7" x="14" y="14" rx="1"></rect>
                                          <rect width="7" height="7" x="3" y="14" rx="1"></rect>
                                      </svg>
                                  """;

    public const string Trash2 = """
                                 <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                         <polyline points="3,6 5,6 21,6"></polyline>
                                         <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                                         <line x1="10" y1="11" x2="10" y2="17"></line>
                                         <line x1="14" y1="11" x2="14" y2="17"></line>
                                     </svg>
                                 """;

    public const string AlertTriangle = """
                                        <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path>
                                                <line x1="12" y1="9" x2="12" y2="13"></line>
                                                <line x1="12" y1="17" x2="12.01" y2="17"></line>
                                            </svg>
                                        """;

    // Additional Category Icons
    public const string Code2 = """
                                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path d="M18 16l4-4-4-4"></path>
                                        <path d="M6 8l-4 4 4 4"></path>
                                        <path d="M14.5 4l-5 16"></path>
                                    </svg>
                                """;

    public const string Briefcase = """
                                    <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <rect width="20" height="14" x="2" y="7" rx="2" ry="2"></rect>
                                            <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"></path>
                                        </svg>
                                    """;

    public const string Cpu = """
                              <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                      <rect width="16" height="16" x="4" y="4" rx="2" ry="2"></rect>
                                      <rect width="6" height="6" x="9" y="9" rx="1" ry="1"></rect>
                                      <path d="M9 1v3"></path>
                                      <path d="M15 1v3"></path>
                                      <path d="M9 20v3"></path>
                                      <path d="M15 20v3"></path>
                                      <path d="M20 9h3"></path>
                                      <path d="M20 14h3"></path>
                                      <path d="M1 9h3"></path>
                                      <path d="M1 14h3"></path>
                                  </svg>
                              """;

    public const string Sparkles = """
                                   <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                           <path d="M9.937 15.5A2 2 0 0 0 8.5 14.063l-6.135-1.382a2 2 0 0 0-1.45 2.914l1.382 6.135a2 2 0 0 0 2.914 1.45l6.135-1.382a2 2 0 0 0 1.45-2.914L9.937 15.5Z"></path>
                                           <path d="M22.5 8.5a2 2 0 0 0-1.5-1.5l-5.5-1.25a2 2 0 0 0-1.5 3l1.25 5.5a2 2 0 0 0 3 1.5l5.5-1.25a2 2 0 0 0 1.5-3L22.5 8.5Z"></path>
                                       </svg>
                                   """;

    // Method to get icon with custom class
    public static string GetIcon(string iconName, string customClass = "")
    {
        return iconName switch
        {
            "menu" => Menu.Replace("h-5 w-5", customClass),
            "close" => Close.Replace("h-5 w-5", customClass),
            "search" => Search.Replace("h-4 w-4", customClass),
            "help" => Help.Replace("h-4 w-4", customClass),
            "moon" => Moon.Replace("h-5 w-5", customClass),
            "sun" => Sun.Replace("h-5 w-5", customClass),
            "heart" => Heart.Replace("h-4 w-4", customClass),
            "history" => History.Replace("h-4 w-4", customClass),
            "plus" => Plus.Replace("h-4 w-4", customClass),
            "download" => Download.Replace("h-4 w-4", customClass),
            "copy" => Copy.Replace("h-4 w-4", customClass),
            "eye" => Eye.Replace("h-4 w-4", customClass),
            "star" => Star.Replace("h-4 w-4", customClass),
            "clock" => Clock.Replace("h-4 w-4", customClass),
            "check" => Check.Replace("h-4 w-4", customClass),
            "alert" => Alert.Replace("h-4 w-4", customClass),
            "file" => File.Replace("h-4 w-4", customClass),
            "upload" => Upload.Replace("h-4 w-4", customClass),
            "grid" => Grid.Replace("h-5 w-5", customClass),
            "calendar" => Calendar.Replace("h-4 w-4", customClass),
            "trash" => Trash.Replace("h-4 w-4", customClass),
            "play" => Play.Replace("h-4 w-4", customClass),
            "code" => Code.Replace("h-4 w-4", customClass),
            "lightbulb" => Lightbulb.Replace("h-4 w-4", customClass),
            "refresh" => Refresh.Replace("h-4 w-4", customClass),
            "layout-grid" => LayoutGrid.Replace("h-4 w-4", customClass),
            "trending-up" => TrendingUp.Replace("h-4 w-4", customClass),
            "pen-tool" => PenTool.Replace("h-4 w-4", customClass),
            "graduation-cap" => GraduationCap.Replace("h-4 w-4", customClass),
            "bar-chart-3" => BarChart3.Replace("h-4 w-4", customClass),
            "play-circle" => PlayCircle.Replace("h-4 w-4", customClass),
            "target" => Target.Replace("h-4 w-4", customClass),
            "zap" => Zap.Replace("h-4 w-4", customClass),
            "x" => X.Replace("h-4 w-4", customClass),
            "file-text" => FileText.Replace("h-4 w-4", customClass),
            "settings" => Settings.Replace("h-4 w-4", customClass),
            "edit" => Edit.Replace("h-4 w-4", customClass),
            "check-circle" => CheckCircle.Replace("h-4 w-4", customClass),
            "refresh-cw" => RefreshCw.Replace("h-4 w-4", customClass),
            "save" => Save.Replace("h-4 w-4", customClass),
            "help-circle" => HelpCircle.Replace("h-4 w-4", customClass),
            "grid-3x3" => Grid3x3.Replace("h-4 w-4", customClass),
            "trash-2" => Trash2.Replace("h-4 w-4", customClass),
            "alert-triangle" => AlertTriangle.Replace("h-4 w-4", customClass),
            // Category icons
            "LayoutGrid" => LayoutGrid.Replace("h-4 w-4", customClass),
            "TrendingUp" => TrendingUp.Replace("h-4 w-4", customClass),
            "Code2" => Code2.Replace("h-4 w-4", customClass),
            "PenTool" => PenTool.Replace("h-4 w-4", customClass),
            "Briefcase" => Briefcase.Replace("h-4 w-4", customClass),
            "GraduationCap" => GraduationCap.Replace("h-4 w-4", customClass),
            "Cpu" => Cpu.Replace("h-4 w-4", customClass),
            "Sparkles" => Sparkles.Replace("h-4 w-4", customClass),
            "Zap" => Zap.Replace("h-4 w-4", customClass),
            "BarChart3" => BarChart3.Replace("h-4 w-4", customClass),
            _ => ""
        };
    }
}
