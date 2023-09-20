using LQ.Wx.Zhang.Common;
using LQ.Wx.Zhang.DAL;

namespace LQ.Wx.Zhang.BLL
{
    public class UserBll : BaseBll<User,UserReq.Page>
    {
        #region 当前状态
        public static User? GetCookie()
        {
            var arr = (HttpContext.Current?.User?.Identity?.Name + "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr == null || arr.Length < 3)
            {
                return null;
            }
            return new User { Id = Convert.ToInt32(arr[0]), Account = arr[1], Name = arr[2] };
        }
        public static User? CurrentUser
        {
            get
            {
                if (!HttpContextCache.CanUse)
                {
                    return null;
                }
                var _currentUser = HttpContextCache.Cache.G<User>("CurrentUser");
                if (_currentUser == null)
                {
                    try
                    {
                        //获取当前登录用户信息
                        var cuser = GetCookie();
                        if (cuser == null)
                        {
                            return null;
                        }
                        var userBll =  HttpContext.GetService<UserBll>();
                        _currentUser = userBll.Find(false, cuser.Id);
                        if (_currentUser == null)
                        {
                            return null;
                        }
                        HttpContextCache.Cache["CurrentUser"] = _currentUser;
                    }
                    catch (Exception)
                    {
                        _currentUser = null;
                    }
                }

                return _currentUser;
            }
        }
        #endregion
    }
}