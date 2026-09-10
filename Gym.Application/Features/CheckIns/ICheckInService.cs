namespace Gym.Application.CheckIns;

public interface ICheckInService
{
    CheckInResult CheckIn(CheckInRequest request);
}
