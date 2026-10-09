namespace Api.DTOs.Gameplay;

public record MoveTokenDto(
    int TokenId,
    double CoordX,
    double CoordY
);

public record TokenPositionDto(
    int TokenId,
    double CoordX,
    double CoordY
);

public record EndSessionRequestDto(
    int MapId,
    List<TokenPositionDto> Tokens
);