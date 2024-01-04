//public virtual void JoinLobbyByID(string lobbyID)
//{
//    LobbySearch search = new LobbySearch();
//    EOSSDKComponent.GetLobbyInterface().CreateLobbySearch(new CreateLobbySearchOptions { MaxResults = 1 }, out search);
//    search.SetLobbyId(new LobbySearchSetLobbyIdOptions { LobbyId = lobbyID });
//
//    search.Find(new LobbySearchFindOptions { LocalUserId = EOSSDKComponent.LocalUserProductId }, null, (LobbySearchFindCallbackInfo callback) => {
//        //If the search was unsuccessful, invoke an error event and return
//        if (callback.ResultCode != Result.Success) {
//            FindLobbiesFailed?.Invoke("There was an error while finding lobbies. Error: " + callback.ResultCode);
//            return;
//        }
//
//        foundLobbies.Clear();
//
//        //for each lobby found, add data to details
//        for (int i = 0; i < search.GetSearchResultCount(new LobbySearchGetSearchResultCountOptions {}); i++) {
//            LobbyDetails lobbyInformation;
//            search.CopySearchResultByIndex(new LobbySearchCopySearchResultByIndexOptions { LobbyIndex = (uint)i }, out lobbyInformation);
//
//            foundLobbies.Add(lobbyInformation);
//        }
//
//        //Invoke event
//        FindLobbiesSucceeded?.Invoke(foundLobbies);
//    });
//}
