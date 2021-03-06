﻿using System.Web.Mvc;

namespace ScrewTurn.Wiki.Web.Models.Common
{
    public class PageNotFoundModel : WikiBaseModel
    {
        public string Description { get; set; }

        public MvcHtmlString SearchResults { get; set; }
    }
}