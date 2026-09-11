using Gym.Domain;

namespace Gym.Application.CheckIns;

public interface ICheckInOperationLogger
{
    void LogSuccessfulCheckIn(CheckIn checkIn);
}
