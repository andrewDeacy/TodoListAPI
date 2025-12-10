using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Moq;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Tests.Services;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<IConfigurationSection> _mockJwtSection = null!;
    private AuthService _authService = null!;

    private const string TestSecretKey = "YourSuperSecretKeyForJWTTokenGenerationMustBeAtLeast32CharactersLong";
    private const string TestIssuer = "TodoListAPI";
    private const string TestAudience = "TodoListAPIUsers";
    private const string TestExpirationMinutes = "60";

    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockJwtSection = new Mock<IConfigurationSection>();

        // Setup JWT configuration section
        _mockJwtSection.Setup(s => s["SecretKey"]).Returns(TestSecretKey);
        _mockJwtSection.Setup(s => s["Issuer"]).Returns(TestIssuer);
        _mockJwtSection.Setup(s => s["Audience"]).Returns(TestAudience);
        _mockJwtSection.Setup(s => s["ExpirationMinutes"]).Returns(TestExpirationMinutes);

        _mockConfiguration.Setup(c => c.GetSection("Jwt")).Returns(_mockJwtSection.Object);

        _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    #region RegisterAsync Tests

    [TestMethod]
    public async Task RegisterAsync_Success_ReturnsTokenAndUserInfo()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "TestPassword123"
        };

        var userId = Guid.NewGuid();
        var createdUser = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = "hashed_password",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Token);
        Assert.AreEqual(userId, result.UserId);
        Assert.AreEqual(request.Email, result.Email);
        Assert.AreEqual(request.Username, result.Username);

        // Verify password was hashed (not stored as plain text)
        _mockUserRepository.Verify(r => r.CreateAsync(It.Is<User>(u => 
            u.Email == request.Email && 
            u.Username == request.Username && 
            u.PasswordHash != request.Password && // Password should be hashed
            !string.IsNullOrEmpty(u.PasswordHash))), Times.Once);

        // Verify JWT token is valid
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);
        Assert.IsNotNull(token);
        Assert.AreEqual(TestIssuer, token.Issuer);
        Assert.AreEqual(TestAudience, token.Audiences.First());
    }

    [TestMethod]
    public async Task RegisterAsync_ThrowsException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Username = "newuser",
            Password = "TestPassword123"
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(request));

        Assert.IsTrue(exception.Message.Contains("already registered"));
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task RegisterAsync_ThrowsException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "new@example.com",
            Username = "existinguser",
            Password = "TestPassword123"
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(request));

        Assert.IsTrue(exception.Message.Contains("already taken"));
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task RegisterAsync_ThrowsException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            () => _authService.RegisterAsync(null!));
    }

    [TestMethod]
    public async Task RegisterAsync_PasswordIsHashed_NotStoredAsPlainText()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "PlainTextPassword123"
        };

        var userId = Guid.NewGuid();
        User? capturedUser = null;

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User u) => u);

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        Assert.IsNotNull(capturedUser);
        Assert.AreNotEqual(request.Password, capturedUser.PasswordHash);
        Assert.IsTrue(capturedUser.PasswordHash.Length > 20); // BCrypt hashes are long
        Assert.IsTrue(capturedUser.PasswordHash.StartsWith("$2")); // BCrypt hash prefix
    }

    [TestMethod]
    public async Task RegisterAsync_UserHasDefaultRole()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "TestPassword123"
        };

        User? capturedUser = null;

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User u) => u);

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        Assert.IsNotNull(capturedUser);
        Assert.AreEqual("User", capturedUser.Role);
    }

    #endregion

    #region LoginAsync Tests

    [TestMethod]
    public async Task LoginAsync_SuccessWithEmail_ReturnsTokenAndUserInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var password = "TestPassword123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = passwordHash,
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            EmailOrUsername = user.Email,
            Password = password
        };

        _mockUserRepository.Setup(r => r.GetByEmailOrUsernameAsync(user.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Token);
        Assert.AreEqual(userId, result.UserId);
        Assert.AreEqual(user.Email, result.Email);
        Assert.AreEqual(user.Username, result.Username);

        // Verify JWT token is valid
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);
        Assert.IsNotNull(token);
    }

    [TestMethod]
    public async Task LoginAsync_SuccessWithUsername_ReturnsTokenAndUserInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var password = "TestPassword123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = passwordHash,
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            EmailOrUsername = user.Username,
            Password = password
        };

        _mockUserRepository.Setup(r => r.GetByEmailOrUsernameAsync(user.Username))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Token);
        Assert.AreEqual(userId, result.UserId);
        Assert.AreEqual(user.Email, result.Email);
        Assert.AreEqual(user.Username, result.Username);
    }

    [TestMethod]
    public async Task LoginAsync_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailOrUsername = "nonexistent@example.com",
            Password = "TestPassword123"
        };

        _mockUserRepository.Setup(r => r.GetByEmailOrUsernameAsync(request.EmailOrUsername))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request));

        Assert.IsTrue(exception.Message.Contains("Invalid email/username or password"));
    }

    [TestMethod]
    public async Task LoginAsync_ThrowsException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var correctPassword = "CorrectPassword123";
        var wrongPassword = "WrongPassword123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = passwordHash,
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            EmailOrUsername = user.Email,
            Password = wrongPassword
        };

        _mockUserRepository.Setup(r => r.GetByEmailOrUsernameAsync(user.Email))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request));

        Assert.IsTrue(exception.Message.Contains("Invalid email/username or password"));
    }

    [TestMethod]
    public async Task LoginAsync_ThrowsException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            () => _authService.LoginAsync(null!));
    }

    [TestMethod]
    public async Task LoginAsync_VerifiesPasswordWithBCrypt()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var password = "TestPassword123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = passwordHash,
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            EmailOrUsername = user.Email,
            Password = password
        };

        _mockUserRepository.Setup(r => r.GetByEmailOrUsernameAsync(user.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.IsNotNull(result);
        // If we get here without exception, password verification succeeded
        // BCrypt.Verify is called internally, so this test verifies the integration works
    }

    #endregion

    #region JWT Token Generation Tests

    [TestMethod]
    public async Task RegisterAsync_GeneratedTokenContainsCorrectClaims()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "TestPassword123"
        };

        var userId = Guid.NewGuid();
        var createdUser = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = "hashed_password",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);

        // Verify claims
        Assert.AreEqual(userId.ToString(), token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.AreEqual(request.Email, token.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.AreEqual(request.Username, token.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.AreEqual("User", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.AreEqual(userId.ToString(), token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.AreEqual(request.Email, token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.IsNotNull(token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti));
    }

    [TestMethod]
    public async Task RegisterAsync_GeneratedTokenHasCorrectExpiration()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "TestPassword123"
        };

        var userId = Guid.NewGuid();
        var createdUser = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = "hashed_password",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);

        var expectedExpiration = DateTime.UtcNow.AddMinutes(int.Parse(TestExpirationMinutes));
        var timeDifference = Math.Abs((token.ValidTo - expectedExpiration).TotalMinutes);

        // Allow 1 minute tolerance for test execution time
        Assert.IsTrue(timeDifference < 1, $"Token expiration should be approximately {TestExpirationMinutes} minutes from now");
        Assert.IsTrue(token.ValidTo > DateTime.UtcNow, "Token should not be expired");
    }

    [TestMethod]
    public async Task RegisterAsync_GeneratedTokenHasCorrectIssuerAndAudience()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "TestPassword123"
        };

        var userId = Guid.NewGuid();
        var createdUser = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = "hashed_password",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);

        Assert.AreEqual(TestIssuer, token.Issuer);
        Assert.AreEqual(TestAudience, token.Audiences.First());
    }

    [TestMethod]
    public async Task RegisterAsync_ThrowsException_WhenJwtSecretKeyNotConfigured()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "TestPassword123"
        };

        var userId = Guid.NewGuid();
        var createdUser = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = "hashed_password",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        // Setup configuration with missing SecretKey
        var mockJwtSectionEmpty = new Mock<IConfigurationSection>();
        mockJwtSectionEmpty.Setup(s => s["SecretKey"]).Returns((string?)null);
        _mockConfiguration.Setup(c => c.GetSection("Jwt")).Returns(mockJwtSectionEmpty.Object);

        var authServiceWithEmptyConfig = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);

        _mockUserRepository.Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => authServiceWithEmptyConfig.RegisterAsync(request));

        Assert.IsTrue(exception.Message.Contains("JWT SecretKey is not configured"));
    }

    #endregion
}
