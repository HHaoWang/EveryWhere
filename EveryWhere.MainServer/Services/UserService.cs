using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using EveryWhere.Database;
using EveryWhere.Database.PO;
using EveryWhere.DTO.Settings;
using EveryWhere.MainServer.Entity.Exception;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Entity.Response;
using EveryWhere.MainServer.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EveryWhere.MainServer.Services;

public class UserService:BaseService<User>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;
    private readonly TokenSettings _tokenSettings;

    public UserService(IConfiguration configuration,
        IHttpClientFactory clientFactory, Repository repository, IOptions<TokenSettings> tokenSettings) : base(repository)
    {
        _configuration = configuration;
        _clientFactory = clientFactory;
        _tokenSettings = tokenSettings.Value;
    }

    private async Task CreateUser(LoginRequest request,
            WechatCode2SessionResponse userInfo,
            HttpClient client,
            User user)
    {
        //判断参数是否缺失
        if (string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            throw new NoNecessaryParameterException("avatarUrl");
        }
        if (string.IsNullOrWhiteSpace(request.NickName))
        {
            throw new NoNecessaryParameterException("nickName");
        }

        //下载头像并保存
        HttpResponseMessage httpResponse = await client.GetAsync(request.AvatarUrl);
        httpResponse.EnsureSuccessStatusCode();

        Stream file = await httpResponse.Content.ReadAsStreamAsync();
        
        string fileName = Path.GetRandomFileName() + ".jpg";
        DirectoryInfo avatarDirectory = FileUtil.GetAvatarDirectory();
        await using FileStream stream = new(Path.Combine(avatarDirectory.FullName, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        await file.DisposeAsync();

        //用户数据持久化
        user.WechatOpenId = userInfo.OpenId;
        user.WechatSessionKey = userInfo.SessionKey;
        user.Avatar = fileName;
        user.NickName = request.NickName;
        user.CreateTime = DateTime.Now;
        user.WechatUnionId = userInfo.UnionId;

        Repository.Add(user);
        await Repository.SaveChangesAsync();
    }

    public async Task<string> GetAuthenticatedTokenAsync(LoginRequest request)
    {
        #region 请求微信数据
        string requestUrl = "https://api.weixin.qq.com/sns/jscode2session"
                            + "?appid=" + _configuration["appId"]
                            + "&secret=" + _configuration["appSecret"]
                            + "&js_code=" + request.UserCode
                            + "&grant_type=authorization_code";
        HttpClient? client = _clientFactory.CreateClient();
        HttpResponseMessage result = await client.GetAsync(requestUrl);
        string content = await result.Content.ReadAsStringAsync();
        WechatCode2SessionResponse? userInfo = JsonConvert.DeserializeObject<WechatCode2SessionResponse>(content);

        //如果请求失败则抛出异常
        if (userInfo?.ErrorCode != 0)
        {
            throw new RequestWechatOpenIdException(userInfo?.ErrorMsg,
                (RequestWechatOpenIdException.ErrorCode?)userInfo?.ErrorCode);
        }
        #endregion

        #region 查找用户
        User? user = await Repository.Users
            !.FirstOrDefaultAsync(user => userInfo.OpenId!.Equals(user.WechatOpenId));
        #endregion

        #region 用户不存在则创建用户
        if (user == null)
        {
            user = new User();
            await CreateUser(request, userInfo, client, user);
        }
        #endregion

        #region 更新session
        //用户session key有变动的话则同步更新至数据库中
        if (!user.WechatSessionKey.Equals(userInfo.SessionKey))
        {
            user.WechatSessionKey = userInfo.SessionKey;
            await Repository.SaveChangesAsync();
        }
        #endregion

        #region 签发token
        Claim[] claims = {
                new(ClaimTypes.Name,user.Id.ToString()),
                new("UserId",user.Id.ToString()),
                new(ClaimTypes.Role,"Consumer")
            };
        //生成盐值凭据
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_tokenSettings.SaltValue));
        //生成签名证书
        SigningCredentials credential = new(securityKey, SecurityAlgorithms.HmacSha256);
        //生成token
        JwtSecurityToken jwtToken = new(
            issuer: _tokenSettings.Issuer,
            audience: _tokenSettings.Audience,
            claims: claims,
            notBefore: null,
            expires: DateTime.Now.AddHours(_tokenSettings.AccessExpiration),
            signingCredentials: credential);
        string? token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        #endregion

        return token;
    }
}