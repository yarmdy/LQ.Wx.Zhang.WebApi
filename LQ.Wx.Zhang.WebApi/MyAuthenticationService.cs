using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.Linq;

namespace LQ.Wx.Zhang.WebApi
{
    public class MyAuthenticationService : AuthenticationService
    {
        private HashSet<ClaimsPrincipal>? _transformCache;

        public MyAuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers, IClaimsTransformation transform, IOptions<AuthenticationOptions> options) : base(schemes, handlers, transform, options)
        {
            Schemes = schemes;
            Handlers = handlers;
            Transform = transform;
            Options = options.Value;
        }
        /// <summary>
        /// Used to lookup AuthenticationSchemes.
        /// </summary>
        public new IAuthenticationSchemeProvider Schemes { get; }

        /// <summary>
        /// Used to resolve IAuthenticationHandler instances.
        /// </summary>
        public new IAuthenticationHandlerProvider Handlers { get; }

        /// <summary>
        /// Used for claims transformation.
        /// </summary>
        public new IClaimsTransformation Transform { get; }

        /// <summary>
        /// The <see cref="AuthenticationOptions"/>.
        /// </summary>
        public new AuthenticationOptions Options { get; }

        
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultAuthenticateSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            // Handlers should not return null, but we'll be tolerant of null values for legacy reasons.
            var result = (await handler.AuthenticateAsync()) ?? AuthenticateResult.NoResult();

            if (result.Succeeded)
            {
                var principal = result.Principal!;
                var doTransform = true;
                _transformCache ??= new HashSet<ClaimsPrincipal>();
                if (_transformCache.Contains(principal))
                {
                    doTransform = false;
                }

                if (doTransform)
                {
                    principal = await Transform.TransformAsync(principal);
                    _transformCache.Add(principal);
                }
                return AuthenticateResult.Success(new AuthenticationTicket(principal, result.Properties, result.Ticket!.AuthenticationScheme));
            }
            return result;
            //return await base.AuthenticateAsync(context, scheme);
        }
        /// <summary>
        /// 质疑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            if (scheme == null)
            {
                var defaultChallengeScheme = await Schemes.GetDefaultChallengeSchemeAsync();
                scheme = defaultChallengeScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultChallengeScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            await handler.ChallengeAsync(properties);
            //await base.ChallengeAsync(context, scheme, properties);
        }
        /// <summary>
        /// 禁止
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            if (scheme == null)
            {
                var defaultForbidScheme = await Schemes.GetDefaultForbidSchemeAsync();
                scheme = defaultForbidScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultForbidScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            await handler.ForbidAsync(properties);
            //await base.ForbidAsync(context, scheme, properties);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="principal"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            if (Options.RequireAuthenticatedSignIn)
            {
                if (principal.Identity == null)
                {
                    throw new InvalidOperationException("SignInAsync when principal.Identity == null is not allowed when AuthenticationOptions.RequireAuthenticatedSignIn is true.");
                }
                if (!principal.Identity.IsAuthenticated)
                {
                    throw new InvalidOperationException("SignInAsync when principal.Identity.IsAuthenticated is false is not allowed when AuthenticationOptions.RequireAuthenticatedSignIn is true.");
                }
            }

            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultSignInSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultSignInScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingSignInHandlerException(scheme);
            }

            var signInHandler = handler as IAuthenticationSignInHandler;
            if (signInHandler == null)
            {
                throw await CreateMismatchedSignInHandlerException(scheme, handler);
            }

            await signInHandler.SignInAsync(principal, properties);
            //await base.SignInAsync(context, scheme, principal, properties);
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultSignOutSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultSignOutScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingSignOutHandlerException(scheme);
            }

            var signOutHandler = handler as IAuthenticationSignOutHandler;
            if (signOutHandler == null)
            {
                throw await CreateMismatchedSignOutHandlerException(scheme, handler);
            }

            await signOutHandler.SignOutAsync(properties);
            //await base.SignOutAsync(context, scheme, properties);
        }

        private async Task<Exception> CreateMissingHandlerException(string scheme)
        {
            var schemes = string.Join(", ", (await Schemes.GetAllSchemesAsync()).Select(sch => sch.Name));

            var footer = $" Did you forget to call AddAuthentication().Add[SomeAuthHandler](\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                return new InvalidOperationException(
                    $"No authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No authentication handler is registered for the scheme '{scheme}'. The registered schemes are: {schemes}." + footer);
        }

        private async Task<string> GetAllSignInSchemeNames()
        {
            return string.Join(", ", (await Schemes.GetAllSchemesAsync())
                .Where(sch => typeof(IAuthenticationSignInHandler).IsAssignableFrom(sch.HandlerType))
                .Select(sch => sch.Name));
        }

        private async Task<Exception> CreateMissingSignInHandlerException(string scheme)
        {
            var schemes = await GetAllSignInSchemeNames();

            // CookieAuth is the only implementation of sign-in.
            var footer = $" Did you forget to call AddAuthentication().AddCookie(\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                return new InvalidOperationException(
                    $"No sign-in authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No sign-in authentication handler is registered for the scheme '{scheme}'. The registered sign-in schemes are: {schemes}." + footer);
        }

        private async Task<Exception> CreateMismatchedSignInHandlerException(string scheme, IAuthenticationHandler handler)
        {
            var schemes = await GetAllSignInSchemeNames();

            var mismatchError = $"The authentication handler registered for scheme '{scheme}' is '{handler.GetType().Name}' which cannot be used for SignInAsync. ";

            if (string.IsNullOrEmpty(schemes))
            {
                // CookieAuth is the only implementation of sign-in.
                return new InvalidOperationException(mismatchError
                    + $"Did you forget to call AddAuthentication().AddCookie(\"Cookies\") and SignInAsync(\"Cookies\",...)?");
            }

            return new InvalidOperationException(mismatchError + $"The registered sign-in schemes are: {schemes}.");
        }

        private async Task<string> GetAllSignOutSchemeNames()
        {
            return string.Join(", ", (await Schemes.GetAllSchemesAsync())
                .Where(sch => typeof(IAuthenticationSignOutHandler).IsAssignableFrom(sch.HandlerType))
                .Select(sch => sch.Name));
        }

        private async Task<Exception> CreateMissingSignOutHandlerException(string scheme)
        {
            var schemes = await GetAllSignOutSchemeNames();

            var footer = $" Did you forget to call AddAuthentication().AddCookie(\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                // CookieAuth is the most common implementation of sign-out, but OpenIdConnect and WsFederation also support it.
                return new InvalidOperationException($"No sign-out authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No sign-out authentication handler is registered for the scheme '{scheme}'. The registered sign-out schemes are: {schemes}." + footer);
        }

        private async Task<Exception> CreateMismatchedSignOutHandlerException(string scheme, IAuthenticationHandler handler)
        {
            var schemes = await GetAllSignOutSchemeNames();

            var mismatchError = $"The authentication handler registered for scheme '{scheme}' is '{handler.GetType().Name}' which cannot be used for {nameof(SignOutAsync)}. ";

            if (string.IsNullOrEmpty(schemes))
            {
                // CookieAuth is the most common implementation of sign-out, but OpenIdConnect and WsFederation also support it.
                return new InvalidOperationException(mismatchError
                    + $"Did you forget to call AddAuthentication().AddCookie(\"Cookies\") and {nameof(SignOutAsync)}(\"Cookies\",...)?");
            }

            return new InvalidOperationException(mismatchError + $"The registered sign-out schemes are: {schemes}.");
        }
    }

    //
    // 摘要:
    //     Implementation of Microsoft.AspNetCore.Authentication.IAuthenticationHandlerProvider.
    public class MyAuthenticationHandlerProvider : IAuthenticationHandlerProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IAuthenticationHandlerProvider"/>.</param>
        public MyAuthenticationHandlerProvider(IAuthenticationSchemeProvider schemes)
        {
            Schemes = schemes;
        }

        /// <summary>
        /// The <see cref="IAuthenticationHandlerProvider"/>.
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; }

        // handler instance cache, need to initialize once per request
        private readonly Dictionary<string, IAuthenticationHandler> _handlerMap = new Dictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);

        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationScheme">The name of the authentication scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        public async Task<IAuthenticationHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
        {
            if (_handlerMap.TryGetValue(authenticationScheme, out var value))
            {
                return value;
            }

            var scheme = await Schemes.GetSchemeAsync(authenticationScheme);
            if (scheme == null)
            {
                return null;
            }
            var type = scheme.HandlerType == typeof(CookieAuthenticationHandler) ? typeof(MyCookieAuthenticationHandler) : scheme.HandlerType;
            var handler = (context.RequestServices.GetService(type) ??
                ActivatorUtilities.CreateInstance(context.RequestServices, type))
                as IAuthenticationHandler;
            if (handler != null)
            {
                await handler.InitializeAsync(scheme, context);
                _handlerMap[authenticationScheme] = handler;
            }
            return handler;
        }
    }

    public class MyCookieAuthenticationHandler : SignInAuthenticationHandler<CookieAuthenticationOptions>
    {
        private const string HeaderValueNoCache = "no-cache";
        private const string HeaderValueNoCacheNoStore = "no-cache,no-store";
        private const string HeaderValueEpocDate = "Thu, 01 Jan 1970 00:00:00 GMT";
        private const string SessionIdClaim = "Microsoft.AspNetCore.Authentication.Cookies-SessionId";

        private bool _shouldRefresh;
        private bool _signInCalled;
        private bool _signOutCalled;

        private DateTimeOffset? _refreshIssuedUtc;
        private DateTimeOffset? _refreshExpiresUtc;
        private string? _sessionKey;
        private Task<AuthenticateResult>? _readCookieTask;
        private AuthenticationTicket? _refreshTicket;

        /// <summary>
        /// Initializes a new instance of <see cref="CookieAuthenticationHandler"/>.
        /// </summary>
        /// <param name="options">Accessor to <see cref="CookieAuthenticationOptions"/>.</param>
        /// <param name="logger">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="encoder">The <see cref="UrlEncoder"/>.</param>
        /// <param name="clock">The <see cref="ISystemClock"/>.</param>
        public MyCookieAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new CookieAuthenticationEvents Events
        {
            get { return (CookieAuthenticationEvents)base.Events!; }
            set { base.Events = value; }
        }

        /// <inheritdoc />
        protected override Task InitializeHandlerAsync()
        {
            // Cookies needs to finish the response
            Context.Response.OnStarting(FinishResponseAsync);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new CookieAuthenticationEvents());

        private Task<AuthenticateResult> EnsureCookieTicket()
        {
            // We only need to read the ticket once
            if (_readCookieTask == null)
            {
                _readCookieTask = ReadCookieTicket();
            }
            return _readCookieTask;
        }

        private async Task CheckForRefreshAsync(AuthenticationTicket ticket)
        {
            var currentUtc = Clock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var allowRefresh = ticket.Properties.AllowRefresh ?? true;
            if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                var eventContext = new CookieSlidingExpirationContext(Context, Scheme, Options, ticket, timeElapsed, timeRemaining)
                {
                    ShouldRenew = timeRemaining < timeElapsed,
                };
                await Options.Events.OnCheckSlidingExpiration(eventContext);

                if (eventContext.ShouldRenew)
                {
                    RequestRefresh(ticket);
                }
            }
        }

        private void RequestRefresh(AuthenticationTicket ticket, ClaimsPrincipal? replacedPrincipal = null)
        {
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (issuedUtc != null && expiresUtc != null)
            {
                _shouldRefresh = true;
                var currentUtc = Clock.UtcNow;
                _refreshIssuedUtc = currentUtc;
                var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
                _refreshExpiresUtc = currentUtc.Add(timeSpan);
                _refreshTicket = CloneTicket(ticket, replacedPrincipal);
            }
        }

        private static AuthenticationTicket CloneTicket(AuthenticationTicket ticket, ClaimsPrincipal? replacedPrincipal)
        {
            var principal = replacedPrincipal ?? ticket.Principal;
            var newPrincipal = new ClaimsPrincipal();
            foreach (var identity in principal.Identities)
            {
                newPrincipal.AddIdentity(identity.Clone());
            }

            var newProperties = new AuthenticationProperties();
            foreach (var item in ticket.Properties.Items)
            {
                newProperties.Items[item.Key] = item.Value;
            }

            return new AuthenticationTicket(newPrincipal, newProperties, ticket.AuthenticationScheme);
        }

        private async Task<AuthenticateResult> ReadCookieTicket()
        {
            var cookie = Options.CookieManager.GetRequestCookie(Context, Options.Cookie.Name!);
            if (string.IsNullOrEmpty(cookie))
            {
                return AuthenticateResult.NoResult();
            }

            var ticket = Options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
            //var ticket = Context.RequestServices.GetRequiredService<ISecureDataFormat<AuthenticationTicket>>().Unprotect(cookie, GetTlsTokenBinding());
            if (ticket == null)
            {
                return AuthenticateResult.Fail("Unprotect ticket failed");
            }

            if (Options.SessionStore != null)
            {
                var claim = ticket.Principal.Claims.FirstOrDefault(c => c.Type.Equals(SessionIdClaim));
                if (claim == null)
                {
                    return AuthenticateResult.Fail("SessionId missing");
                }
                // Only store _sessionKey if it matches an existing session. Otherwise we'll create a new one.
                ticket = await Options.SessionStore.RetrieveAsync(claim.Value, Context.RequestAborted);
                if (ticket == null)
                {
                    return AuthenticateResult.Fail("Identity missing in session store");
                }
                _sessionKey = claim.Value;
            }

            var currentUtc = Clock.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                if (Options.SessionStore != null)
                {
                    await Options.SessionStore.RemoveAsync(_sessionKey!, Context.RequestAborted);
                }
                return AuthenticateResult.Fail("Ticket expired");
            }

            // Finally we have a valid ticket
            return AuthenticateResult.Success(ticket);
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await EnsureCookieTicket();
            if (!result.Succeeded)
            {
                return result;
            }

            // We check this before the ValidatePrincipal event because we want to make sure we capture a clean clone
            // without picking up any per-request modifications to the principal.
            await CheckForRefreshAsync(result.Ticket);

            Debug.Assert(result.Ticket != null);
            var context = new CookieValidatePrincipalContext(Context, Scheme, Options, result.Ticket);
            await Events.ValidatePrincipal(context);

            if (context.Principal == null)
            {
                return AuthenticateResult.Fail("No principal.");
            }

            if (context.ShouldRenew)
            {
                RequestRefresh(result.Ticket, context.Principal);
            }

            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
        }

        private CookieOptions BuildCookieOptions()
        {
            var cookieOptions = Options.Cookie.Build(Context);
            // ignore the 'Expires' value as this will be computed elsewhere
            cookieOptions.Expires = null;

            return cookieOptions;
        }

        /// <inheritdoc />
        protected virtual async Task FinishResponseAsync()
        {
            // Only renew if requested, and neither sign in or sign out was called
            if (!_shouldRefresh || _signInCalled || _signOutCalled)
            {
                return;
            }

            var ticket = _refreshTicket;
            if (ticket != null)
            {
                var properties = ticket.Properties;

                if (_refreshIssuedUtc.HasValue)
                {
                    properties.IssuedUtc = _refreshIssuedUtc;
                }

                if (_refreshExpiresUtc.HasValue)
                {
                    properties.ExpiresUtc = _refreshExpiresUtc;
                }

                if (Options.SessionStore != null && _sessionKey != null)
                {
                    await Options.SessionStore.RenewAsync(_sessionKey, ticket, Context.RequestAborted);
                    var principal = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[] { new Claim(SessionIdClaim, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
                            Scheme.Name));
                    ticket = new AuthenticationTicket(principal, null, Scheme.Name);
                }

                var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
                //var cookieValue = Context.RequestServices.GetRequiredService<ISecureDataFormat<AuthenticationTicket>>().Protect(ticket, GetTlsTokenBinding());

                var cookieOptions = BuildCookieOptions();
                if (properties.IsPersistent && _refreshExpiresUtc.HasValue)
                {
                    cookieOptions.Expires = _refreshExpiresUtc.Value.ToUniversalTime();
                }

                Options.CookieManager.AppendResponseCookie(
                    Context,
                    Options.Cookie.Name!,
                    cookieValue,
                    cookieOptions);

                await ApplyHeaders(shouldRedirectToReturnUrl: false, properties: properties);
            }
        }

        /// <inheritdoc />
        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            properties = properties ?? new AuthenticationProperties();

            _signInCalled = true;

            // Process the request cookie to initialize members like _sessionKey.
            await EnsureCookieTicket();
            var cookieOptions = BuildCookieOptions();

            var signInContext = new CookieSigningInContext(
                Context,
                Scheme,
                Options,
                user,
                properties,
                cookieOptions);

            DateTimeOffset issuedUtc;
            if (signInContext.Properties.IssuedUtc.HasValue)
            {
                issuedUtc = signInContext.Properties.IssuedUtc.Value;
            }
            else
            {
                issuedUtc = Clock.UtcNow;
                signInContext.Properties.IssuedUtc = issuedUtc;
            }

            if (!signInContext.Properties.ExpiresUtc.HasValue)
            {
                signInContext.Properties.ExpiresUtc = issuedUtc.Add(Options.ExpireTimeSpan);
            }

            await Events.SigningIn(signInContext);

            if (signInContext.Properties.IsPersistent)
            {
                var expiresUtc = signInContext.Properties.ExpiresUtc ?? issuedUtc.Add(Options.ExpireTimeSpan);
                signInContext.CookieOptions.Expires = expiresUtc.ToUniversalTime();
            }

            var ticket = new AuthenticationTicket(signInContext.Principal!, signInContext.Properties, signInContext.Scheme.Name);

            if (Options.SessionStore != null)
            {
                if (_sessionKey != null)
                {
                    // Renew the ticket in cases of multiple requests see: https://github.com/dotnet/aspnetcore/issues/22135
                    await Options.SessionStore.RenewAsync(_sessionKey, ticket, Context.RequestAborted);
                }
                else
                {
                    _sessionKey = await Options.SessionStore.StoreAsync(ticket, Context.RequestAborted);
                }

                var principal = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[] { new Claim(SessionIdClaim, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
                        Options.ClaimsIssuer));
                ticket = new AuthenticationTicket(principal, null, Scheme.Name);
            }
            
            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
            //var cookieValue = Context.RequestServices.GetRequiredService<ISecureDataFormat<AuthenticationTicket>>().Protect(ticket, GetTlsTokenBinding());

            Options.CookieManager.AppendResponseCookie(
                Context,
                Options.Cookie.Name!,
                cookieValue,
                signInContext.CookieOptions);

            var signedInContext = new CookieSignedInContext(
                Context,
                Scheme,
                signInContext.Principal!,
                signInContext.Properties,
                Options);

            await Events.SignedIn(signedInContext);

            // Only redirect on the login path
            var shouldRedirect = Options.LoginPath.HasValue && OriginalPath == Options.LoginPath;
            await ApplyHeaders(shouldRedirect, signedInContext.Properties);

            //Logger.AuthenticationSchemeSignedIn(Scheme.Name);
        }

        /// <inheritdoc />
        protected override async Task HandleSignOutAsync(AuthenticationProperties? properties)
        {
            properties = properties ?? new AuthenticationProperties();

            _signOutCalled = true;

            // Process the request cookie to initialize members like _sessionKey.
            await EnsureCookieTicket();
            var cookieOptions = BuildCookieOptions();
            if (Options.SessionStore != null && _sessionKey != null)
            {
                await Options.SessionStore.RemoveAsync(_sessionKey, Context.RequestAborted);
            }

            var context = new CookieSigningOutContext(
                Context,
                Scheme,
                Options,
                properties,
                cookieOptions);

            await Events.SigningOut(context);

            Options.CookieManager.DeleteCookie(
                Context,
                Options.Cookie.Name!,
                context.CookieOptions);

            // Only redirect on the logout path
            var shouldRedirect = Options.LogoutPath.HasValue && OriginalPath == Options.LogoutPath;
            await ApplyHeaders(shouldRedirect, context.Properties);

            //Logger.AuthenticationSchemeSignedOut(Scheme.Name);
        }

        private async Task ApplyHeaders(bool shouldRedirectToReturnUrl, AuthenticationProperties properties)
        {
            Response.Headers.CacheControl = HeaderValueNoCacheNoStore;
            Response.Headers.Pragma = HeaderValueNoCache;
            Response.Headers.Expires = HeaderValueEpocDate;

            if (shouldRedirectToReturnUrl && Response.StatusCode == 200)
            {
                // set redirect uri in order:
                // 1. properties.RedirectUri
                // 2. query parameter ReturnUrlParameter
                //
                // Absolute uri is not allowed if it is from query string as query string is not
                // a trusted source.
                var redirectUri = properties.RedirectUri;
                if (string.IsNullOrEmpty(redirectUri))
                {
                    redirectUri = Request.Query[Options.ReturnUrlParameter];
                    if (string.IsNullOrEmpty(redirectUri) || !IsHostRelative(redirectUri))
                    {
                        redirectUri = null;
                    }
                }

                if (redirectUri != null)
                {
                    await Events.RedirectToReturnUrl(
                        new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, redirectUri));
                }
            }
        }

        private static bool IsHostRelative(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (path.Length == 1)
            {
                return path[0] == '/';
            }
            return path[0] == '/' && path[1] != '/' && path[1] != '\\';
        }

        /// <inheritdoc />
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var returnUrl = properties.RedirectUri;
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = OriginalPathBase + OriginalPath + Request.QueryString;
            }
            var accessDeniedUri = Options.AccessDeniedPath + QueryString.Create(Options.ReturnUrlParameter, returnUrl);
            var redirectContext = new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(accessDeniedUri));
            await Events.RedirectToAccessDenied(redirectContext);
        }

        /// <inheritdoc />
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var redirectUri = properties.RedirectUri;
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
            }

            var loginUri = Options.LoginPath + QueryString.Create(Options.ReturnUrlParameter, redirectUri);
            var redirectContext = new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(loginUri));
            await Events.RedirectToLogin(redirectContext);
        }

        private string? GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }

    public class MyTicketDataFormatMyTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        
        /// <inheritdoc />
        public string Protect(AuthenticationTicket data)
        {
            return Protect(data, purpose: null);
        }

        /// <inheritdoc />
        public string Protect(AuthenticationTicket data, string? purpose)
        {
            string res;
            try
            {
                var bytes = TicketSerializer.Default.Serialize(data);
                res = string.Join("", bytes.Select(a=>a.ToString("X2")));
            }catch (Exception ex)
            {
                res = "";
            }
            return res;
        }

        /// <inheritdoc />
        public AuthenticationTicket? Unprotect(string? protectedText)
        {
            return Unprotect(protectedText, purpose: null);
        }

        /// <inheritdoc />
        public AuthenticationTicket? Unprotect(string? protectedText, string? purpose)
        {
            if (protectedText == null) { 
                return null;
            }
            AuthenticationTicket? res;
            try {
                int i = 0;
                var bytes = protectedText.GroupBy(a => i++ / 2).Select(a => byte.Parse(string.Join("", a),System.Globalization.NumberStyles.HexNumber)).ToArray();
                res = TicketSerializer.Default.Deserialize(bytes);
            }
            catch (Exception ex) {
                res = null;
            }
            return res;
        }

    }

    public class MyDataProtectionProvider : IDataProtectionProvider
    {
        private static IDataProtector _dataProtector = default!;
        
        public IDataProtector CreateProtector(string purpose)
        {
            if (_dataProtector == null)
            {
                _dataProtector = new MyDataProtector();
            }
            return _dataProtector.CreateProtector(purpose);
        }
    }
    public class MyDataProtector : IDataProtector
    {
        private static IDataProtector _dataProtector=default!;
        public IDataProtector CreateProtector(string purpose)
        {
            if (_dataProtector == null)
            {
                _dataProtector = new MyDataProtector();
            }
            return _dataProtector;
        }

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}
