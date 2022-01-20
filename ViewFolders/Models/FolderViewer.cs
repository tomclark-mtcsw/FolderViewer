namespace ViewFolders.Models
{
    public class FolderViewer
    {
        /*
         * Author:  Tom Clark
         * Date:    January 30, 2020
         *
         * This class takes a path as an input and displays the directory
         * structure of where the path points.  Each directory can be expanded.
         * Once expanded it appears in the expandedList string that gets
         * passed in via the client as a pipe-delimited list.  The paths for
         * all items are from the DocumentRootPath and down from there.
         * The full path is never used for security purposes.  Each file can
         * be downloaded or other action taken once customized.
         *
         * The file structure is only filled as the user requests it.  So if
         * a given directory has dozens of nested directories, they are only
         * exposed and retrieved as the user requests.  This keeps the performance
         * reasonable.  The structure is stored in a recursive class.
         */
        private string RootPath;
        private List<string> ExpandedDirectories;
        private DisplayDirectory Contents;
        private string AppDocPath;

        public FolderViewer(string inputpath, string appDocPath, string expandList = "")
        {
            RootPath = inputpath;
            AppDocPath = appDocPath;
            ExpandedDirectories = new List<string>();

            // root directory is always expanded
            if (string.IsNullOrEmpty(expandList))
            {
                expandList = inputpath;
            }

            if (!string.IsNullOrEmpty(expandList))
            {
                var splitlist = expandList.Split('|');
                foreach (var item in splitlist)
                {
                    ExpandedDirectories.Add(item);
                }
            }
            Contents = GetDirectory(AppDocPath + "\\" + inputpath);
        }

        public string Display()
        {
            string output = DirectoryOutput(Contents);
            output += "<input type=\"hidden\" id=\"ExpandedDirectories\" value=\"" + ExpandedDirectoriesString() + "\" />";
            output += "<input type=\"hidden\" id=\"RootPath\" value=\"" + RootPath + "\" />";
            return output;
        }

        private string ExpandedDirectoriesString()
        {
            string output = "";

            foreach (var dir in ExpandedDirectories)
            {
                if (!string.IsNullOrEmpty(dir))
                {
                    if (!string.IsNullOrEmpty(output))
                    {
                        output += "|";
                    }

                    output += dir;
                }
            }

            return output;
        }

        private DisplayDirectory GetDirectory(string path)
        {
            DisplayDirectory output = new();
            output.Name = Path.GetFileName(path);
            output.FullPath = path.Replace(AppDocPath + "\\", "");
            output.Expanded = false;
            output.Files = GetFiles(path);
            output.ItemCount = output.Files.Count;
            output.Directories = new List<DisplayDirectory>();

            if (ExpandedDirectories.Contains(path.Replace(AppDocPath + "\\", "")))
            {
                output.Expanded = true;
            }

            var directories = Directory.GetDirectories(path);
            foreach (var dir in directories)
            {
                output.ItemCount++;
                if (ExpandedDirectories.Contains(dir.Replace(AppDocPath + "\\", "")))
                {
                    output.Directories.Add(GetDirectory(dir));
                }
                else
                {
                    DisplayDirectory nonExpandedDirectory = new()
                    {
                        Name = Path.GetFileName(dir),
                        FullPath = dir.Replace(AppDocPath + "\\", ""),
                        Expanded = false,
                        Files = new List<DisplayFile>(),
                        Directories = new List<DisplayDirectory>(),
                        ItemCount = Directory.GetDirectories(dir).Length +
                                    Directory.GetFiles(dir).Length
                    };
                    output.Directories.Add(nonExpandedDirectory);
                }
            }

            return output;
        }

        private List<DisplayFile> GetFiles(string path)
        {
            List<DisplayFile> output = new();
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    output.Add(new DisplayFile()
                    {
                        FullPath = file.Replace(AppDocPath + "\\", ""),
                        Filename = Path.GetFileName(file),
                        Filesize = fileInfo.Length
                    });
                }
            }

            return output;
        }

        private string DirectoryOutput(DisplayDirectory currentDirectory)
        {
            string output = "";
            if (currentDirectory.Expanded)
            {
                if (currentDirectory.FullPath == RootPath)
                {
                    output += "<div class=\"folder-view-expanded-directory\">";
                    output += "<img src=\"/images/folder_Closed_16xLG.png\" class=\"folder-view-folder-icon\" /> ";
                    output += currentDirectory.Name + "</div>";
                }
                else
                {
                    output += "<div class=\"folder-view-expanded-directory\"><a href=\"javascript:void(0);\" class=\"folder-view-folder-link\" onclick=\"DecreaseFolder('" + currentDirectory.FullPath.Replace("\\", "\\\\") + "')\">- ";
                    output += "<img src=\"/images/folder_Closed_16xLG.png\" class=\"folder-icon\" /> ";
                    output += currentDirectory.Name + "</a></div>";
                }

                output += "<div class=\"folder-view-indent\">";
                foreach (var dir in currentDirectory.Directories)
                {
                    output += DirectoryOutput(dir);
                }

                foreach (var file in currentDirectory.Files)
                {
                    output += "<div class=\"folder-view-file\">&nbsp;<img src=\"/images/document_16xLG.png\" class=\"folder-view-document-icon\" />";
                    output += "<a href=\"javascript:void(0)\" class=\"folder-view-file-link\" />" + file.Filename + "</a>";
                    output += " (" + FileSizeDisplay(file.Filesize) + ")";
                    output += "</div>";
                }

                output += "</div>";
            }
            else
            {
                output += "<div class=\"folder-view-expandable-directory\"><a href=\"javascript:void(0);\" class=\"folder-view-folder-link\" onclick=\"ExpandFolder('" + currentDirectory.FullPath.Replace("\\", "\\\\") + "')\">+ ";
                output += "<img src=\"/images/folder_Closed_16xLG.png\" class=\"folder-view-folder-icon\" /> ";
                output += currentDirectory.Name + "</a>";
                output += " (" + (currentDirectory.ItemCount) + ")";
                output += "</div>";
            }

            return output;
        }

        private static string FileSizeDisplay(long filesize)
        {
            string output = "";

            if (filesize >= 0 && filesize < 1000000)
            {
                output = Math.Round((double)filesize / 1000, 2, MidpointRounding.AwayFromZero).ToString() + " Kb";
            }
            else if (filesize >= 1000000 && filesize < 1000000000)
            {
                output = Math.Round((double)filesize / 1000000, 2, MidpointRounding.AwayFromZero).ToString() + " Mb";
            }
            else if (filesize >= 1000000000)
            {
                output = Math.Round((double)filesize / 1000000000, 2, MidpointRounding.AwayFromZero).ToString() + " Gb";
            }

            return output;
        }
    }

    public class DisplayDirectory
    {
        public List<DisplayDirectory> Directories { get; set; } = new();
        public List<DisplayFile> Files { get; set; } = new();
        public bool Expanded;
        public string FullPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ItemCount;
    }

    public class DisplayFile
    {
        public string FullPath { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public long Filesize;
    }
}
