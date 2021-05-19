import React from "react";
import { hot } from "react-hot-loader/root";
import { Button, IconButton } from "@chakra-ui/button";
import { useColorMode } from "@chakra-ui/color-mode";
import { Flex, Spacer, VStack } from "@chakra-ui/layout";
import { FaSun, FaMoon } from "react-icons/fa"
import { useDispatch, useSelector } from "react-redux";
import { bindActionCreators } from "redux";
import { actionCreators, State } from "../state";

import "./style.css";

function App() {
  const dispatch = useDispatch();
  const { logIn, logOut } = bindActionCreators(actionCreators, dispatch);
  const userAuthState = useSelector((state: State) => state.userAuthState);

  const { colorMode, toggleColorMode } = useColorMode();
  const isDark = colorMode === 'dark';

  return (
    <VStack p="2">
      <Flex w="100%" p="1" border="1px" borderRadius={12}>
        <Spacer />
        { userAuthState ? <Button style={{borderRadius: 20,}} onClick={() => logOut()}>LogOut</Button> : <div></div>}
        <IconButton ml="8" aria-label="color theme switch" onClick={toggleColorMode} icon={isDark ? <FaSun /> : <FaMoon />}/>
      </Flex>
    </VStack>
  );
}

export default hot(App);
