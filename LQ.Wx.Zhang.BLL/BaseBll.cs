using LQ.Wx.Zhang.DAL;
using LQ.Wx.Zhang.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LQ.Wx.Zhang.BLL
{
    public class BaseBll<T, PageReqT> where T : BaseEntity where PageReqT:PageReq
    {
        protected BaseBll() {
            Context = Common.HttpContext.Current!.RequestServices.GetRequiredService<ZhangDb>();
        }
        #region 属性
        public int CurrentUserId => UserBll.GetCookie()?.Id ?? 0;
        public string? CurrentUserName => UserBll.GetCookie()?.Name;
        public string? CurrentAccount => UserBll.GetCookie()?.Account;
        #endregion
        #region 依赖
        /// <summary>
        /// 上下文
        /// </summary>
        public ZhangDb Context { get; set; } = default!;

        public EnumDeleteFilterMode DelFilterMode { get; set; } = EnumDeleteFilterMode.Normal;
        #endregion

        #region 抽象方法

        #endregion

        #region 分页
        public virtual Expression<Func<T, bool>> PageWhereKeyword(PageReqT req)
        {
            var where = (Expression<Func<T, bool>>)(a => false);
            if (string.IsNullOrEmpty(req.keyword))
            {
                return where.Or(a => true);
            }

            if (TypeHelper.HasPropertyBase<T>("Title"))
            {
                where = where.Or(PredicateBuilder.DotExpression<T, string>("Title").Like(req.keyword));
            }
            if (TypeHelper.HasPropertyBase<T>("Name"))
            {
                where = where.Or(PredicateBuilder.DotExpression<T, string>("Name").Like(req.keyword));
            }
            if (TypeHelper.HasPropertyBase<T>("Remark"))
            {
                where = where.Or(PredicateBuilder.DotExpression<T, string>("Mobile").Like(req.keyword));
            }
            if (TypeHelper.HasPropertyBase<T>("Keys"))
            {
                var tkey = req.keyword.Replace("|", "");
                tkey = $"|{tkey}|";
                where = where.Or(PredicateBuilder.DotExpression<T, string>("Keys").Like(tkey));

            }
            where = where.Or(a => a.CreateUser!.Name.Contains(req.keyword));
            return where;
        }
        /// <summary>
        /// 分页条件
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IQueryable<T> PageWhere(PageReqT req, IQueryable<T> query)
        {
            query = query.Where(PageWhereKeyword(req));
            if (req.Where != null)
            {
                foreach (var where in req.Where)
                {
                    if (!TypeHelper.HasProperty<T>(where.Key))
                    {
                        query = PageWhereCustom(req, query, where);
                        continue;
                    }
                    query = query.EqualTo(where.Key, where.Value);
                }
            }
            return query;
        }
        public virtual IQueryable<T> PageWhereCustom(PageReqT req, IQueryable<T> query, KeyValuePair<string, string> where)
        {
            return query;
        }
        public virtual IQueryable<T> DefOrderBy(PageReqT? req, IOrderedQueryable<T> query)
        {
            if (TypeHelper.HasPropertyBase<T>("Id"))
            {
                return query.ThenByDescending("Id");
            }
            return query.ThenByDescending(a => a.CreateTime);
        }
        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IQueryable<T> PageOrder(PageReqT req, IQueryable<T> query)
        {
            IOrderedQueryable<T> sortQuery = query.OrderBy(a => 0);
            if (req.Sort != null)
            {
                foreach (var sort in req.Sort)
                {
                    if (!TypeHelper.HasProperty<T>(sort.Key))
                    {
                        query = PageOrderCustom(req, query, sort);
                        continue;
                    }
                    if (sort.Value)
                    {
                        sortQuery = sortQuery.ThenBy(sort.Key);
                    }
                    else
                    {
                        sortQuery = sortQuery.ThenByDescending(sort.Key);
                    }
                }
            }
            return DefOrderBy(req, sortQuery);
        }
        public virtual IQueryable<T> PageOrderCustom(PageReqT req, IQueryable<T> query, KeyValuePair<string, bool> sort)
        {
            return query;
        }
        /// <summary>
        /// 分页完成后处理数据
        /// </summary>
        /// <param name="req"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual void PageAfter(PageReqT req, Response<List<T>, object, object, object> res)
        {

        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Response<List<T>, object, object, object> GetPageList(PageReqT req)
        {
            var res = new Response<List<T>, object, object, object>();
            var list = GetListPage(out int count, req.Page, req.Size, a => PageWhere(req, a), a => PageOrder(req, a));

            res.count = count;
            res.data = list;
            PageAfter(req, res);
            return res;
        }
        #endregion

        #region 获取详情页数据
        public virtual bool DetailBefore(int id, int? id2, Response<T, object, object, object> res)
        {
            return true;
        }
        public virtual void DetailAfter(int id, int? id2, Response<T, object, object, object> res)
        {

        }
        public Response<T, object, object, object> GetDetail(int id, int? id2)
        {
            var res = new Response<T, object, object, object>();

            var beforeRes = DetailBefore(id, id2, res);
            if (!beforeRes)
            {
                return res;
            }
            var obj = Find(false, id, id2);
            if (obj == null)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "您查询的数据不存在";
            }
            res.data = obj;
            DetailAfter(id, id2, res);
            return res;
        }
        #endregion

        #region 新增

        public virtual bool AddValidate(out string errorMsg, T model)
        {
            errorMsg = "";
            return true;
        }
        public virtual bool AddBefore(out string errorMsg, T model, T inModel)
        {
            errorMsg = "";
            return true;
        }
        public virtual void AddAfter(Response<T, object, object, object> res, T inModel)
        {

        }
        public Response<T, object, object, object> Add(T model)
        {
            var res = new Response<T, object, object, object>();
            var tmpModel = model;
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (!AddValidate(out string errorMsg, model))
            {
                res.code = EnumResStatus.Fail;
                res.msg = errorMsg;
                return res;
            }
            var values = getKeyValues(model);
            var hasOld = false;
            if (model is IdEntity)
            {
                if ((model as IdEntity)!.Id != 0)
                {
                    throw new ArgumentNullException("model.Id");
                }
                //(model as IdEntity)!.Id = -1;
                Context.Set<T>().Add(model);
            }
            else
            {
                var tmp = Find(false, values.Select(a => a.Value).ToArray());
                if (tmp != null)
                {
                    hasOld = true;
                    //Context.Configuration.LazyLoadingEnabled = false;
                    tmp.CopyFrom(model, a => new { a.CreateTime, a.CreateUserId }, new[] { typeof(BaseEntity), typeof(ICollection<>) });
                    //Context.Configuration.LazyLoadingEnabled = true;
                    model = tmp;
                }
                else
                {
                    Context.Set<T>().Add(model);
                }
            }
            if (hasOld)
            {
                model.ModifyTime = DateTime.Now;
                model.ModifyUserId = CurrentUserId;
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.CreateUserId = CurrentUserId;
            }
            if (!AddBefore(out errorMsg, model, tmpModel))
            {
                res.code = EnumResStatus.Fail;
                res.msg = errorMsg;
                return res;
            }

            var ret = Context.SaveChanges();
            if (ret <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "新增失败";
            }
            res.data = model;
            AddAfter(res, tmpModel);

            return res;
        }
        #endregion

        #region 修改
        public virtual bool ModifyValidate(out string errorMsg, T model)
        {
            errorMsg = "";
            return true;
        }
        public virtual bool ModifyBefore(out string errorMsg, T model, T inModel, T oldModel)
        {
            errorMsg = "";
            return true;
        }
        public virtual void ModifyAfter(Response<T, object, object, object> res, T inModel, T oldModel)
        {

        }
        public virtual Expression<Func<T, object>>? ModifyExcepts(T model)
        {
            return null;
        }
        public Response<T, object, object, object> Modify(T model)
        {
            var res = new Response<T, object, object, object>();
            var tmpModel = model;
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (!ModifyValidate(out string errorMsg, model))
            {
                res.code = EnumResStatus.Fail;
                res.msg = errorMsg;
                return res;
            }

            var values = getKeyValues(model);

            if (values.Any(a => a.Value == null || a.Value.Equals(0)))
            {
                throw new ArgumentNullException($"model.{string.Join(",", values.Keys)}");
            }
            var old = Find(true, values.Select(a => a.Value).ToArray())!;
            var tmp = Find(false, values.Select(a => a.Value).ToArray());
            if (tmp == null)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "找不到要修改的数据";
                return res;
            }
            //Context.Configuration.LazyLoadingEnabled = false;
            var expr = ModifyExcepts(model);
            var exprList = new List<string>();
            if (expr != null)
            {
                exprList.AddRange(expr.GetProperties().Select(a => a.Name));
            }
            exprList.AddRange(new[] { nameof(model.CreateTime), nameof(model.CreateUserId), nameof(model.ModifyTime), nameof(model.ModifyUserId), nameof(model.DelTime), nameof(model.DelUserId) });
            if (tmp.DiffCopyExcept(model, exprList.ToArray(), new[] { typeof(BaseEntity), typeof(ICollection<>) }))
            {
                tmp.ModifyTime = DateTime.Now;
                tmp.ModifyUserId = CurrentUserId;
            }
            //Context.Configuration.LazyLoadingEnabled = true;
            model = tmp;


            if (!ModifyBefore(out errorMsg, model, tmpModel, old))
            {
                res.code = EnumResStatus.Fail;
                res.msg = errorMsg;
                return res;
            }

            var ret = Context.SaveChanges();
            if (ret < 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "修改失败";
            }
            else if (ret == 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "没有任何修改";
            }
            res.data = model;
            ModifyAfter(res, tmpModel, old);
            return res;
        }
        #endregion

        #region 修改个别字段
        public Response<T, object, object, object> EditProperties(int id, int? id2, object obj)
        {
            var res = new Response<T, object, object, object>();
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var properties = TypeHelper.GetProperties(obj.GetType());
            if (properties.Length <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "未传入要修改的数据";
                return res;
            }

            var model = Find(false, id, id2);
            if (model == null)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "要修改的数据不存在";
                return res;
            }
            model.CopyFrom(obj);
            model.ModifyTime = DateTime.Now;
            model.ModifyUserId = CurrentUserId;


            var ret = Context.SaveChanges();
            if (ret <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "修改失败";
                return res;
            }
            res.data = model;
            return res;

        }
        public Response<T, object, object, object> EditProperties(int id, int? id2, EditPartsReq req)
        {
            var res = new Response<T, object, object, object>();
            if ((req.Properties?.Count ?? 0) <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "未传入要修改的数据";
                return res;
            }
            var existsProperties = req.Properties!.Where(a => TypeHelper.HasPropertyBase<T>(a.Key)).ToDictionary(a => a.Key, a => a.Value);
            if (existsProperties.Count <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "传入的数据无法修改";
                return res;
            }
            var model = Find(false, id, id2);
            if (model == null)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "要修改的数据不存在";
                return res;
            }
            model.ModifyTime = DateTime.Now;
            model.ModifyUserId = CurrentUserId;

            existsProperties.ForEach(a => {
                var prop = TypeHelper.GetPropertyBase<T>(a.Key);
                model.SetPropertyValue(a.Key, Convert.ChangeType(a.Value, prop!.PropertyType));
            });
            var ret = Context.SaveChanges();
            if (ret <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "修改失败";
                return res;
            }
            res.data = model;
            return res;
        }
        #endregion

        #region 删除
        public virtual bool DeleteValidate(out string errorMsg, List<T> models, int[][] ids)
        {
            errorMsg = "";
            return true;
        }
        public virtual void DeleteAfter(Response<T, object, object, object> res, int[][] ids)
        {

        }
        public Response<T, object, object, object> Delete(int[][] ids)
        {
            var res = new Response<T, object, object, object>();
            if (ids == null || ids.Length <= 0 || ids[0].Length <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "没有要删除的数据";
                return res;
            }
            var names = getKeyNames();
            var models = ids.Select(a => {
                var model = (T)Activator.CreateInstance(typeof(T))!;
                var i = 0;
                names.ToList().ForEach(b => TypeHelper.SetPropertyValue(model, b, a[i++]));
                return model;
            }).ToList();
            if (DeleteValidate(out string errorMsg, models, ids))
            {
                res.code = EnumResStatus.Fail;
                res.msg = errorMsg;
                return res;
            }
            models.ForEach(model => {
                Context.Set<T>().Attach(model);
                Context.Entry(model).State = EntityState.Deleted;
            });
            var ret = Context.SaveChanges();
            if (ret <= 0)
            {
                res.code = EnumResStatus.Fail;
                res.msg = "删除失败";
                return res;
            }
            res.data2 = ret;
            DeleteAfter(res, ids);
            return res;
        }
        #endregion

        #region lambda查询
        private IQueryable<T> getListQuery(Func<IQueryable<T>, IQueryable<T>>? where, bool notracking)
        {
            IQueryable<T> query;
            if (notracking)
            {
                query = Context.Set<T>().AsNoTracking().AsQueryable();
            }
            else
            {
                query = Context.Set<T>().AsQueryable();
            }
            query = where == null ? query : where(query);
            switch (DelFilterMode)
            {
                case EnumDeleteFilterMode.Normal:
                    {
                        query = query.Where(a => !a.IsDel);
                    }
                    break;
                case EnumDeleteFilterMode.Deleted:
                    {
                        query = query.Where(a => a.IsDel);
                    }
                    break;
                case EnumDeleteFilterMode.All:
                    { }
                    break;
            }
            return query;
        }

        public List<T> GetListPage(out int count, int page, int size, Func<IQueryable<T>, IQueryable<T>>? where = null, Func<IQueryable<T>, IQueryable<T>>? order = null, bool notracking = false)
        {
            var query = getListQuery(where, notracking);

            count = query.Count();
            if (count == 0)
            {
                return new List<T>();
            }
            if (order == null)
            {
                query = DefOrderBy(null,query.OrderBy(a => 0));
            }
            else
            {
                query = order(query);
            }
            return query.Skip(size * (page - 1)).Take(size).ToList();
        }
        public List<T> GetListFilter(Func<IQueryable<T>, IQueryable<T>> where)
        {
            return GetListFilter(where, null, true);
        }
        public List<T> GetListFilter(Func<IQueryable<T>, IQueryable<T>>? where, Func<IQueryable<T>, IQueryable<T>>? order, bool notracking)
        {
            var query = getListQuery(where, notracking);

            if (order == null)
            {
                query = query.OrderByDescending(a => a.CreateTime);
            }
            else
            {
                query = order(query);
            }
            return query.ToList();
        }
        public T? GetFirstOrDefault(Func<IQueryable<T>, IQueryable<T>>? where, bool notracking = true)
        {
            return GetFirstOrDefault(where, null, notracking);
        }
        public T? GetFirstOrDefault(Func<IQueryable<T>, IQueryable<T>>? where, Func<IQueryable<T>, IQueryable<T>>? order, bool notracking)
        {
            var query = getListQuery(where, notracking);

            if (order == null)
            {
                query = query.OrderByDescending(a => a.CreateTime);
            }
            else
            {
                query = order(query);
            }
            return query.FirstOrDefault();
        }
        public List<T> ByIds(int[] Ids, Expression<Func<T, int>>? expr = null)
        {
            return ByIds(true, Ids, expr);
        }
        public List<T> ByIds(bool notracking, int[] Ids, Expression<Func<T, int>>? expr)
        {
            if (!typeof(T).IsSubclassOf(typeof(IdEntity)) && expr == null || Ids == null || Ids.Length <= 0)
            {
                return new List<T>();
            }
            if (typeof(T).IsSubclassOf(typeof(IdEntity)))
            {
                expr = PredicateBuilder.DotExpression<T, int>("Id");
            }
            var where = PredicateBuilder.DotExpression<T, int>("Id").In(Ids);
            return GetListFilter(a => a.Where(where), a => a.OrderByDescending(b => b.CreateTime), notracking);
        }
        public T? Find(params object[] keys)
        {
            return Find(true, keys);
        }
        public T? Find(bool notracking, params object?[] keys)
        {
            if (keys == null) return null;
            keys = keys.Where(a => a != null).ToArray();
            if (keys.Length <= 0) return null;
            keys = convertKeyType(keys);
            keys = keys.Select(a => a).ToArray();
            var obj = Context.Set<T>().Find(keys);
            if (obj == null)
            {
                return obj;
            }
            if (notracking)
            {
                Context.Entry(obj).State = EntityState.Detached;
            }
            switch (DelFilterMode)
            {
                case EnumDeleteFilterMode.Normal:
                    {
                        obj = obj.IsDel ? null : obj;
                    }
                    break;
                case EnumDeleteFilterMode.Deleted:
                    {
                        obj = (!obj.IsDel) ? null : obj;
                    }
                    break;
                case EnumDeleteFilterMode.All:
                    { }
                    break;
            }
            return obj;
        }
        private object[] convertKeyType(params object?[] keys)
        {
            var objset = Context.Model.FindEntityType(typeof(T));
            //var objset = (Context as IObjectContextAdapter).ObjectContext.CreateObjectSet<T>();
            int i = 0;
            var keyTypes = objset!.GetKeys().Where(a => a.IsPrimaryKey()).Select(a => {
                var type = a.GetKeyType();
                var res = Convert.ChangeType(keys[i], type, null);
                i++;
                return res;
            }).ToArray();
            return keyTypes!;
        }

        private string[] getKeyNames()
        {
            //var stt = (Context as IObjectContextAdapter).ObjectContext.CreateObjectSet<T>();
            var stt = Context.Model.FindEntityType(typeof(T));
            return stt!.GetKeys().Where(a => a.IsPrimaryKey()).Select(a => a.Properties.First().Name).ToArray()!;
        }
        private Dictionary<string, object?> getKeyValues(T model)
        {
            var names = getKeyNames();
            if (names.Length <= 0)
            {
                throw new Exception("系统错误，主键获取失败");
            }
            return names.ToDictionary(a => a, a => TypeHelper.GetPropertyValueObj(model, a));
        }

        /// <summary>
        /// include一下
        /// </summary>
        /// <returns></returns>
        protected static IQueryable<T> GetIncludeQuery<TKey>(IQueryable<T> query, Expression<Func<T, TKey>> includeSelector)
        {
            if (includeSelector == null)
            {
                return query;
            }
            if (includeSelector.Body.NodeType != ExpressionType.New && includeSelector.Body.NodeType != ExpressionType.MemberAccess)
            {
                return query;
            }
            if (includeSelector.Body.NodeType == ExpressionType.MemberAccess)
            {
                var path = getMembersPath(includeSelector.Body);
                if (string.IsNullOrEmpty(path)) return query;
                return query.Include(path);
            }
            foreach (var arg in ((NewExpression)includeSelector.Body).Arguments)
            {
                var path = getMembersPath(arg);
                if (string.IsNullOrEmpty(path)) continue;
                query = query.Include(path);
            }
            return query;
        }
        /// <summary>
        /// 获取include路径
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static string? getMembersPath(Expression expr)
        {
            if (!(expr is MemberExpression)) return null;
            var exprList = new List<Expression> { expr };
            var path = "";
            while (exprList.Count > 0)
            {
                var last = exprList[exprList.Count - 1];
                exprList.RemoveAt(exprList.Count - 1);
                if (last is MemberExpression)
                {
                    var memExpr = (last as MemberExpression)!;
                    path = "." + memExpr.Member.Name + path;
                    exprList.Add(memExpr.Expression!);
                }
                else
                {
                    var children = last.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(a => a.PropertyType.IsAssignableFrom(typeof(Expression))).OrderBy(a => a.PropertyType.IsAssignableFrom(typeof(MemberExpression)) ? 1 : 0).Select(a => (Expression)a.GetValue(last)!).ToList();
                    exprList.AddRange(children);
                }

            }
            if (path.StartsWith("."))
            {
                path = path.Substring(1);
            }
            return path;
        }
        #endregion
    }

    public static class IServiceEx
    {
        public static IServiceCollection AddBll(this IServiceCollection services)
        {
            services.AddScoped<ZhangDb>();
            Assembly.GetExecutingAssembly().GetTypes().Where(a=>a.BaseType!=null && a.BaseType.IsGenericType && a.BaseType.GetGenericTypeDefinition()==typeof(BaseBll<,>)).ToList().ForEach(a => {
                services.AddScoped(a);
            });
            return services;
        }
    }

    public static class BaseEntityEx
    {
        public static T LoadChild<T>(this T obj, Func<T, object> func) where T : BaseEntity
        {
            if (func != null)
            {
                obj.ObjectTag = func(obj);
            }
            //var context = AutofacExt.GetService<TestContext>();
            //(context as IObjectContextAdapter).ObjectContext.ContextOptions.LazyLoadingEnabled = false;

            return obj;
        }
    }
}