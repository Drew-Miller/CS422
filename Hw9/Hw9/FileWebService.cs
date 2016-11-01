using System;
using System.Text;
using System.IO;

namespace CS422
{
	public class FileWebService : WebService
	{
		private readonly FileSys422 r_sys;

		public FileWebService(FileSys422 fs)
		{
			r_sys = fs;
		}

		public override void Handler(WebRequest req)
		{
			if(!req.URI.StartsWith(ServiceURI, StringComparison.Ordinal))
			{
				throw new InvalidOperationException();
			}

			//split the path
			string[] places = req.URI.Substring(ServiceURI.Length).Split('/');
			Dir422 dir = r_sys.GetRoot();
			for (int i = 0; i < places.Length - 1; i++)
			{
				dir = dir.GetDir(places[i]);
				if(dir == null)
				{
					req.WriteNotFoundResponse("");
					return;
				}
			}

			File422 file = dir.GetFile(places[places.Length - 1]);

			if(file != null)
			{
				RespondWithFile(file, req);
			}

			else
			{
				dir = dir.GetDir(places[places.Length - 1]);
				if(dir!=null)
				{
					RespondWithList(dir, req);
				}

				else
				{
					req.WriteNotFoundResponse("");
				}
			}
		}

		//when a directory is requested
		private void RespondWithList(Dir422 dir, WebRequest req)
		{
			var html = new System.Text.StringBuilder("<html>");

			//DIR STUFF
			html.Append("<h1>Folders</h1>");

			foreach(Dir422 directory in dir.GetDirs())
			{
				html.Append(BuildDirHTML(directory));
			}


			//FILE STUFF
			html.Append("<h1>Files</h1>");

			foreach(File422 file in dir.GetFiles())
			{
				html.AppendFormat(BuildFileHTML(file));
			}

			html.AppendLine("</html>");
			req.WriteHtmlResponse(html.ToString());
		}

		//when a file is requested
		private void RespondWithFile(File422 file, WebRequest req)
		{
			StringBuilder sb = new StringBuilder();


			string[] extension = file.Name.Split('.');

			if(extension[1] == "png")
			{
				req.Type = "image/png";
			}

			else if (extension[1] == "jpg")
			{
				req.Type = "image/jpg";
			}

			else if (extension[1] == "pdf")
			{
				req.Type = "application/pdf";
			}

			else if (extension[1] == "mp4")
			{
				req.Type = "video/mp4";
			}

			else if (extension[1] == "mp3")
			{
				req.Type = "video/mp3";
			}

			else if (extension[1] == "html")
			{
				req.Type = "text/html";
			}

			else if (extension[1] == "XML")
			{
				req.Type = "text/xml";
			}

			else
			{
				req.Type = "text";
			}


			req.WriteFileResponse(file.OpenReadOnly());
		}

		//used to build the dirHtml
		private string BuildDirHTML(Dir422 directory)
		{
			StringBuilder html = new StringBuilder();
			string link = directory.Name;

			//recurse through all parent directories until roo
			//also, all spaces are replaced with %20

			Dir422 parent = directory.Parent;

			//since the root has a null parent, we iterate until we are there
			//once we are, we append the /files/ HTML request
			while (parent.Name != r_sys.GetRoot().Name && parent.Name != "")
			{
				link = parent.Name + "/" + link;
				parent = parent.Parent;
			}

			link = "/files/" + link;


			html.AppendFormat(
					"<a href=\"{0}\">{1}</a><br>",
					link,
					directory.Name
				);

			//get HREF for File422 object:
			//last part FILE422.Name
			//recurse through parent directories until hitting root
			//for each one, append directory name to FRONT of string

			return html.ToString();
		}

		//used to build the files html
		private string BuildFileHTML(File422 file)
		{
			StringBuilder html = new StringBuilder();
			string link = file.Name;

			//recurse through all parent directories until roo
			//also, all spaces are replaced with %20

			Dir422 parent = file.Parent;

			//since the root has a null parent, we iterate until we are there
			//once we are, we append the /files/ HTML request
			while(parent.Name != r_sys.GetRoot().Name)
			{
				link = parent.Name + "/" + link;
				parent = parent.Parent;
			}

			link = "/files/" + link;


			html.AppendFormat(
					"<a href=\"{0}\">{1}</a><br>",
					link,
					file.Name
				);

			//get HREF for File422 object:
			//last part FILE422.Name
			//recurse through parent directories until hitting root
			//for each one, append directory name to FRONT of string

			return html.ToString();
		}

		public override string ServiceURI
		{
			get
			{
				return "/files";
			}
		}
	}
}
