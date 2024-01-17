public abstract class HttpServlet
{
    public abstract void DoGet(HttpServletRequest request, HttpServletResponse response);

    public abstract void DoPost(HttpServletRequest request, HttpServletResponse response);
}
