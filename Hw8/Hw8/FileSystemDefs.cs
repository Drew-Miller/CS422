using System;
using System.Collections.Generic;
using System.IO;

namespace CS422
{
	public abstract class Dir422
	{
		public abstract string Name { get; }

		public abstract IList<Dir422> GetDirs();
		public abstract IList<File422> GetFiles();

		public abstract Dir422 Parent { get; }

		public abstract bool ContainsFile(string fileName, bool recursive);
		public abstract bool ContainsDir(string dirnName, bool recursive);

		public abstract File422 GetFile(string name);
		public abstract Dir422 GetDir(string name);

		public abstract File422 CreateFile(string name);
		public abstract Dir422 CreateDir(string name);
	}

	public abstract class File422
	{
		public abstract string Name { get; }
		public abstract Dir422 Parent { get; }

		//this stream that we get better not support writing
		//can write must be false
		public abstract Stream OpenReadOnly();
		public abstract Stream OpenReadWrite();
	}

	public abstract class FileSys422
	{
		public abstract Dir422 GetRoot();

		public virtual bool Contains(File422 file)
		{
			if (file.Parent != null)
			{
				return Contains(file.Parent);
			}

			else
			{
				return GetRoot().ContainsFile(file.Name, false);
			}
		}

		public virtual bool Contains(Dir422 dir)
		{
			if (dir == null) { return false;}

			while(dir != GetRoot())
			{
				if(dir == null)
				{
					return false;
				}

				dir = dir.Parent;
			}

			return true;
		}
	}


	public class StdFSDir :Dir422
	{
		private string a_path;
		private string _name;
		bool _root;

		public StdFSDir(string path)
		{
			a_path = path;
			_root = false;
			_name = System.IO.Path.GetFileName(path);
		}

		public StdFSDir(string path, bool r)
		{
			a_path = path;
			_root = r;
			string name = new System.IO.DirectoryInfo(a_path).Name;
			_name = name;
		}

		public override string Name
		{
			get
			{
				return _name;
			}
		}

		public override Dir422 Parent
		{
			get
			{
				if (!_root)
				{
					var dirInfo = new DirectoryInfo(a_path);
					return new StdFSDir(dirInfo.Parent.FullName);
				}

				else
				{
					return null;
				}
			}
		}

		public override bool ContainsDir(string dirnName, bool recursive)
		{
			if (dirnName.Contains("/") || dirnName.Contains("\\"))
			{
				return false;
			}

			string mod = a_path + "/" + dirnName;

			if (!recursive)
			{
				foreach (string dir in Directory.GetDirectories(a_path))
				{
					if (String.Equals(mod, dir))
					{
						return true;
					}
				}
			}

			if (recursive)
			{
				foreach (string dir in Directory.GetDirectories(a_path))
				{
					if (String.Equals(mod, dir))
					{
						return true;
					}
				}

				//if we have dirs to traverse to
				if(GetDirs().Count > 0)
				{
					foreach(Dir422 d in GetDirs())
					{
						if(d.ContainsDir(dirnName, true))
						{
							return true;
						}
					}
				}

				return false;
			}

			return false;
		}

		public override bool ContainsFile(string fileName, bool recursive)
		{
			if (fileName.Contains("/") || fileName.Contains("\\"))
			{
				return false;
			}

			string mod = a_path + "/" + fileName;

			if (!recursive)
			{
				foreach (string file in Directory.GetFiles(a_path))
				{
					if (String.Equals(mod, file))
					{
						return true;
					}
				}
			}

			if(recursive)
			{
				foreach (string dir in Directory.GetFiles(a_path))
				{
					if (String.Equals(mod, dir))
					{
						return true;
					}
				}

				//if we have dirs to traverse to
				if (GetDirs().Count > 0)
				{
					foreach (Dir422 d in GetDirs())
					{
						if(d.ContainsFile(fileName, true))
						{
							return true;
						}
					}
				}

				return false;
			}

			return false;
		}

		public override Dir422 CreateDir(string name)
		{
			if (name == null) { return null; }

			if (name.Contains("/") || name.Contains("\\") || name == null)
			{
				return null;
			}

			if (!Directory.Exists(a_path + "/" + name))
			{
				Directory.CreateDirectory(a_path + "/" + name);
				return new StdFSDir(a_path + "/" + name);
			}

			else
			{
				return new StdFSDir(a_path + "/" + name);
			}
		}

		public override File422 CreateFile(string name)
		{
			if (name == null) { return null;}

			if(name.Contains("/") || name.Contains("\\") || name == null)
			{
				return null;
			}

			if(!File.Exists(a_path + "/" + name))
			{
				var fs = File.Create(a_path + "/" + name);
				fs.Close();
				return new StdFSFile(a_path + "/" + name);
			}

			else
			{
				FileStream fs = File.OpenWrite(a_path + "/" + name);
				fs.SetLength(0);
				fs.Close();

				return new StdFSFile(a_path + "/" + name);
			}
		}

		public override Dir422 GetDir(string name)
		{
			if(name.Contains("/") || name.Contains("\\"))
			{
				return null;
			}

			foreach (string dir in Directory.GetDirectories(a_path))
			{
				string n = new System.IO.DirectoryInfo(dir).Name;
				if (String.Equals(name, n))
				{
					return new StdFSDir(a_path + "/" + name);
				}
			}

			return null;
		}

		public override IList<Dir422> GetDirs()
		{
			IList<Dir422> dirs = new List<Dir422>();

			foreach (string file in Directory.GetDirectories(a_path))
			{
				dirs.Add(new StdFSDir(file));
			}

			return dirs;
		}

		public override File422 GetFile(string name)
		{
			if (name.Contains("/") || name.Contains("\\"))
			{
				return null;
			}

			foreach(string file in Directory.GetFiles(a_path))
			{
				string n = new System.IO.DirectoryInfo(file).Name;
				if(String.Equals(n, name))
				{
					return new StdFSFile(a_path + "/" + name);
				}
			}

			return null;
		}

		public override IList<File422> GetFiles()
		{
			IList<File422> files = new List<File422>();

			foreach(string file in Directory.GetFiles(a_path))
			{
				files.Add(new StdFSFile(file));
			}

			return files;
		}
	}

	public class StdFSFile:File422
	{
		private string a_path;
		private string _name;
		FileStream fs;

		public StdFSFile(string path)
		{
			a_path = path;
			_name = new System.IO.DirectoryInfo(System.IO.Path.GetFileName(path)).Name;
		}

		public override string Name
		{
			get
			{
				return _name;
			}
		}

		public override Dir422 Parent
		{
			get
			{
				var dirInfo = new DirectoryInfo(a_path);
				return new StdFSDir(dirInfo.Parent.FullName);
			}
		}

		public override Stream OpenReadOnly()
		{
			return File.OpenRead(a_path);
		}

		public override Stream OpenReadWrite()
		{
			return new FileStream(a_path, FileMode.Open, FileAccess.ReadWrite, FileShare.None); 
		}
	}

	public class StandardFileSystem : FileSys422
	{
		private Dir422 _root;
		private string a_path;

		public override Dir422 GetRoot()
		{
			return _root;
		}

		public static StandardFileSystem Create(string rootDir)
		{
			if (Directory.Exists(rootDir))
			{
				return new StandardFileSystem(new StdFSDir(rootDir, true), rootDir);
			}
			else
			{
				return null;
			}
		}

		private StandardFileSystem(Dir422 root, string path)
		{
			a_path = path;
			_root = root;
		}

		public override bool Contains(Dir422 dir)
		{
			if (dir == null) { return false; }

			while (dir != GetRoot())
			{
				if (dir.Name == GetRoot().Name)
				{
					return true;
				}

				dir = dir.Parent;
			}

			return false;
		}

		public override bool Contains(File422 file)
		{
			return Contains(file.Parent);
		}
	}


	public class MemoryFileSystem : FileSys422
	{
		MemFSDir _root; 

		public MemoryFileSystem()
		{
			_root = new MemFSDir("root", null);
		}

		public override Dir422 GetRoot()
		{
			return _root;
		}
	}

	public class MemFSDir : Dir422
	{
		private string _name;
		private MemFSDir _parent;
		private IList<MemFSDir> Directories;
		private IList<MemFSFile> Files;

		public MemFSDir(string name, MemFSDir parent)
		{
			_parent = parent;
			_name = name;

			Directories = new List<MemFSDir>();
			Files = new List<MemFSFile>();
		}

		public override string Name
		{
			get
			{
				return _name;
			}
		}

		public override Dir422 Parent
		{
			get
			{
				return _parent;
			}
		}

		public override bool ContainsDir(string dirnName, bool recursive)
		{
			if(recursive)
			{
				foreach(MemFSDir d in Directories)
				{
					if(d.Name == dirnName)
					{
						return true;
					}
				}

				foreach(MemFSDir d in Directories)
				{
					if(d.ContainsDir(dirnName, true))
					{
						return true;
					}
				}

				return false;
			}

			else
			{
				foreach (MemFSDir d in Directories)
				{
					if (d.Name == dirnName)
					{
						return true;
					}
				}

				return false;
			}
		}

		public override bool ContainsFile(string fileName, bool recursive)
		{
			if (recursive)
			{
				foreach (MemFSFile f in Files)
				{
					if (f.Name == fileName)
					{
						return true;
					}
				}

				foreach (MemFSDir d in Directories)
				{
					if (d.ContainsFile(fileName, true))
					{
						return true;
					}
				}

				return false;
			}

			else
			{
				foreach (MemFSFile f in Files)
				{
					if (f.Name == fileName)
					{
						return true;
					}
				}

				return false;
			}
		}

		public override Dir422 CreateDir(string name)
		{
			bool contains = false;

			foreach(MemFSDir d in Directories)
			{
				if(d.Name == name)
				{
					contains = true;
				}
			}

			if(contains)
			{
				return null;
			}

			MemFSDir newDir = new MemFSDir(name, this);
			Directories.Add(newDir);
			return newDir;
		}

		public override File422 CreateFile(string name)
		{
			bool contains = false;

			foreach (MemFSFile f in Files)
			{
				if (f.Name == name)
				{
					contains = true;
				}
			}

			if (contains)
			{
				return null;
			}

			MemFSFile newFile = new MemFSFile(name, this);
			Files.Add(newFile);
			return newFile;
		}

		public override Dir422 GetDir(string name)
		{
			if (name == null || name.Contains("/") || name.Contains("\\"))
				return null;

			foreach(MemFSDir d in Directories)
			{
				if(d.Name == name)
				{
					return d;
				}
			}

			return null;
		}

		public override IList<Dir422> GetDirs()
		{
			return (IList<Dir422>)Directories;
		}

		public override File422 GetFile(string name)
		{
			if (name == null || name.Contains("/") || name.Contains("\\"))
				return null;

			foreach (MemFSFile f in Files)
			{
				if (f.Name == name)
				{
					return f;
				}
			}

			return null;
		}

		public override IList<File422> GetFiles()
		{
			return (IList<File422>)Files;
		}
	}

	public class MemFSFile : File422
	{
		private string _name;
		private MemFSDir _parent;
		private byte[] _data;

		public MemFSFile(string name, MemFSDir parent)
		{
			_name = name;
			_parent = parent;
			_data = new byte[256];
		}

		public override string Name
		{
			get
			{
				return _name;
			}
		}

		public override Dir422 Parent
		{
			get
			{
				return _parent;
			}
		}

		public override Stream OpenReadOnly()
		{
			return new MemoryStream(_data);
		}

		public Stream OpenWriteOnly()
		{
			return new MemoryStream(_data);
		}

		public override Stream OpenReadWrite()
		{
			return new MemoryStream(_data);
		}
	}
}