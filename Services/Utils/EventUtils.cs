using HaBuddies.Models;
using System;

namespace HaBuddies.Services.Utils
{
    public static class EventUtil {
        public static bool IsOwnedEvent(UserNoPassword userObj, Event evt) {
            if (userObj != null) {
                if (evt.OwnerId.ToString() == userObj.Id.ToString()) return true;
            }
            return false;
        }

        public static bool IsJoinable(UserNoPassword userObj, Event evt) {
            if (userObj != null) {
                if (evt.MinAgeRequirement != null && evt.MaxAgeRequirement != null) {
                    if (userObj.Age >= evt.MinAgeRequirement && userObj.Age <= evt.MaxAgeRequirement) {
                        return false;
                    }
                }
                if (!evt.GenderRequirement.Any(s => s.Contains(userObj.Gender.ToString())) || evt.IsOpen == false) {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool IsCurrentlyJoined(UserNoPassword userObj, Event evt) {
            if (userObj != null) {
                if (evt.SubscribersId.Any(s => s.Contains(userObj.Id.ToString()))) return true;
            }
            return false;
        }

    }
}