using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AdminQuallyMVC.ViewModels;

namespace AdminQuallyMVC.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }
        public string PageAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            if (PageModel.HasPreviousPage)
            {
                TagBuilder prevItem = CreatePrev(PageModel.PageNumber - 1, urlHelper);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder currentItem = CreateTag(i, urlHelper);
                tag.InnerHtml.AppendHtml(currentItem);
            }

            if (PageModel.HasNextPage)
            {
                TagBuilder nextItem = CreateNext(PageModel.PageNumber + 1, urlHelper);
                tag.InnerHtml.AppendHtml(nextItem);
            }
            output.Content.AppendHtml(tag);
        }

        TagBuilder CreatePrev(int prev, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            item.AddCssClass("page-item");
            TagBuilder link = new TagBuilder("a");
            link.AddCssClass("page-link");
            link.Attributes["href"] = urlHelper.Action(PageAction, new { page = prev });
            link.Attributes.Add("aria-label", "Previous");
            TagBuilder span = new TagBuilder("span");
            span.Attributes.Add("aria-hidden", "true");
            span.InnerHtml.Append("«");
            TagBuilder span2 = new TagBuilder("span");
            span2.AddCssClass("sr-only");
            span2.InnerHtml.Append("Previous");
            link.InnerHtml.AppendHtml(span);
            link.InnerHtml.AppendHtml(span2);
            item.InnerHtml.AppendHtml(link);
            return item;
        }

        TagBuilder CreateNext(int next, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            item.AddCssClass("page-item");
            TagBuilder link = new TagBuilder("a");
            link.AddCssClass("page-link");
            link.Attributes["href"] = urlHelper.Action(PageAction, new { page = next });
            link.Attributes.Add("aria-label", "Next");
            TagBuilder span = new TagBuilder("span");
            span.Attributes.Add("aria-hidden", "true");
            span.InnerHtml.Append("»");
            TagBuilder span2 = new TagBuilder("span");
            span2.AddCssClass("sr-only");
            span2.InnerHtml.Append("Next");
            link.InnerHtml.AppendHtml(span);
            link.InnerHtml.AppendHtml(span2);
            item.InnerHtml.AppendHtml(link);
            return item;
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            item.AddCssClass("page-item");
            TagBuilder link = new TagBuilder("a");
            link.AddCssClass("page-link");
            if (pageNumber == this.PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber });
            }
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
