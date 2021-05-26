import React from "react";
import { hot } from "react-hot-loader/root";
import { Button, IconButton } from "@chakra-ui/button";
import { useColorMode } from "@chakra-ui/color-mode";
import { Flex, Heading, Spacer, VStack } from "@chakra-ui/layout";
import { FaSun, FaMoon } from "react-icons/fa";
import { useDispatch, useSelector } from "react-redux";
import { bindActionCreators } from "redux";
import { actionCreators, AuthenticatedUser, ChannelJoined } from "../state";

import "./style.css";
import LogInOrSignup from "./auth/LogInOrSignUp";
import MainPage from "./MainPage/MainPage";

function App() {
  const dispatch = useDispatch();
  const { logOut, channelJoined, removeHubChat } = bindActionCreators(
    actionCreators,
    dispatch,
  );
  const userAuthState = useSelector(
    (state: AuthenticatedUser) => state.authenticatedUser,
  );

  const currentChannelJoined = useSelector(
    (state: ChannelJoined) => state.channelJoined,
  );

  const { colorMode, toggleColorMode } = useColorMode();
  const isDark = colorMode === "dark";

  return (
    <VStack p="2" minH="100vh" justifyContent="start">
      <Flex w="100%" p="1" border="1px" borderRadius={4}>
        <Spacer />
        {currentChannelJoined !== "" ? (
          <Heading size="md" color="teal.500" disabled={true}>
            Channel: {currentChannelJoined}
          </Heading>
        ) : null}
        <Spacer />
        {userAuthState ? (
          <Button
            style={{ borderRadius: 16 }}
            onClick={() => {
              logOut();
              channelJoined("");
              removeHubChat();
            }}
          >
            Log out, {userAuthState.userName}
          </Button>
        ) : null}
        <IconButton
          ml="2"
          aria-label="color theme switch"
          onClick={toggleColorMode}
          icon={isDark ? <FaSun /> : <FaMoon />}
        />
      </Flex>
      {userAuthState ? <MainPage /> : <LogInOrSignup />}
    </VStack>
  );
}

export default hot(App);
