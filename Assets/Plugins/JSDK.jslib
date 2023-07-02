var plugin = {
  Loaded: function () {},
  SaveExtern: function (data) {
    console.group("SaveExtern");
    var dataString = UTF8ToString(data);
    var jsonUserData = JSON.parse(dataString);

    player.setData(jsonUserData);

    console.log(jsonUserData);
    console.groupEnd();
  },
  LoadExtern: function () {
    player.getData().then((jsonUserData) => {
      const stringUserData = JSON.stringify(jsonUserData);

      console.group("LoadExtern");
      console.log(jsonUserData);
      console.log(stringUserData);
      console.groupEnd();

      myGameInstance.SendMessage(
        "DataManager",
        "SetPlayerData",
        stringUserData
      );
    });
  },
  GetUserInfoExtern: function () {
    const jsonUserInfo = {
      name: player.getName(),
      photo: player.getPhoto("medium"),
    };

    const stringUserInfo = JSON.stringify(jsonUserInfo);

    console.group("GetUserInfoExtern");
    console.log(jsonUserInfo);
    console.groupEnd();

    myGameInstance.SendMessage("DataManager", "SetUserInfo", stringUserInfo);
  },
  GetLang: function () {
    var lang = ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(lang) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(lang, buffer, bufferSize);

    console.log("GetLang ", lang);

    return buffer;
  },
  SetToLeaderBoard: function (value) {
    console.log("SetToLeaderBoard", value);
    ysdk.getLeaderboards().then((lb) => {
      ysdk
        .isAvailableMethod("leaderboards.setLeaderboardScore")
        .then((status) => {
          if (status) {
            lb.setLeaderboardScore("Rate", value);
          }
        });
    });
  },
  GetLeaderBoard: function () {
    const result = {};
    ysdk.getLeaderboards().then((lb) => {
      // Получение 10 топов и 3 записей возле пользователя
      lb.getLeaderboardEntries("Rate", {
        quantityTop: 10,
        includeUser: true,
        quantityAround: 3,
      }).then((res) => {
        result.leaderboard = {
          title: [],
        };
        for (var key of Object.keys(res.leaderboard.title)) {
          result.leaderboard.title.push({
            lang: key,
            value: res.leaderboard.title[key],
          });
        }

        result.userRank = res.userRank;
        result.entries = [];

        for (let i = 0; i < res.entries.length; i++) {
          const userData = res.entries[i];
          result.entries.push({
            rank: userData.rank,
            score: userData.score,
            name: userData.player.publicName,
            lang: userData.player.lang,
            photo: userData.player.getAvatarSrc("middle"),
          });
        }

        console.group("GetLeaderBoard");
        console.log(result);
        console.groupEnd();

        const stringResult = JSON.stringify(result);
        myGameInstance.SendMessage(
          "DataManager",
          "GetLeaderBoard",
          stringResult
        );
      });
    });
  },
  AddCoinsExtern: function (value) {
    console.log("AddCoinsExtern", value);

    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: () => {
          console.log("Video ad open.");
        },
        onRewarded: () => {
          console.log("Rewarded add coin!");
          myGameInstance.SendMessage("DataManager", "AddCoins", value);
        },
        onClose: () => {
          console.log("Video ad closed.");
        },
        onError: (e) => {
          console.log("Error while open video ad:", e);
        },
      },
    });
  },
  AddHintExtern: function (data) {
    var dataString = UTF8ToString(data);
    console.log("AddHintExtern", dataString);

    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: () => {
          console.log("Video ad open.");
        },
        onRewarded: () => {
          console.log("Rewarded from hint!");
          var dataJson = JSON.parse(dataString);
          const stringResponse = JSON.stringify(dataJson);

          myGameInstance.SendMessage("DataManager", "AddHint", stringResponse);
        },
        onClose: () => {
          console.log("Video ad closed.");
        },
        onError: (e) => {
          console.log("Error while open video ad:", e);
        },
      },
    });
  },
  AddBonusExtern: function (data) {
    var dataString = UTF8ToString(data);
    console.log("AddBonusExtern", dataString);

    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: () => {
          console.log("Video ad open.");
        },
        onRewarded: () => {
          console.log("Rewarded from bonus!");
          var dataJson = JSON.parse(dataString);
          const stringResponse = JSON.stringify(dataJson);

          myGameInstance.SendMessage("DataManager", "AddBonus", stringResponse);
        },
        onClose: () => {
          console.log("Video ad closed.");
        },
        onError: (e) => {
          console.log("Error while open video ad:", e);
        },
      },
    });
  },
  ShowAdvFullScreen: function () {
    ysdk.adv.showFullscreenAdv({
      callbacks: {
        onClose: function (wasShown) {
          // some action after close
        },
        onError: function (error) {
          // some action on error
        },
      },
    });
  },
};

mergeInto(LibraryManager.library, plugin);
