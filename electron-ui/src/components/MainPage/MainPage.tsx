import {
  Box,
  Flex,
  Heading,
  IconButton,
  List,
  ListItem,
  Spacer,
} from "@chakra-ui/react";
import { CgEnter } from "react-icons/cg";
import React from "react";
import { useDispatch, useSelector } from "react-redux";
import { bindActionCreators } from "redux";
import ChatHubConnection from "../../api/hubConnecton";
import { actionCreators, ChannelsReceived, ChatHub } from "../../state";
import AddChannel from "./AddChannel";

const MainPage = (): JSX.Element => {
  const dispatch = useDispatch();
  const { setHubChat } = bindActionCreators(actionCreators, dispatch);
  const channels = useSelector((state: ChannelsReceived) => {
    return state.channels;
  });

  const chatHub = useSelector((state: ChatHub) => {
    return state.chatHub;
  });

  const hub = new ChatHubConnection();
  async function createHubConnection() {
    hub
      .createHubConnection()
      .then(() => {
        setHubChat(hub);
      })
      .catch((err) => {
        console.log(err);
      });
  }

  if (chatHub === null) {
    createHubConnection();
  }

  return (
    <Flex style={{ height: "calc(100vh - 75px)", width: "100%" }}>
      <Box
        border="1px"
        maxW="22vw"
        borderRadius="16"
        flex="1"
        textAlign="center"
      >
        <Spacer h="5px" />
        <Heading color="teal.500" size="md">
          Channels
        </Heading>
        <Spacer h="5px" />
        <AddChannel />
        <Spacer h="5px" />
        <List
          border="1px"
          borderRadius="4"
          marginRight="1"
          marginLeft="1"
          paddingTop="1"
          paddingBottom="1"
          spacing={3}
          overflow="scroll"
          style={{ maxHeight: "calc(100vh - 190px)" }}
        >
          {channels.map((e, i) => {
            return (
              <ListItem
                border="1px"
                borderRadius="8"
                paddingLeft="2"
                paddingRight="2"
                marginLeft="1"
                marginRight="1"
                textAlign="left"
                justifyContent="end"
                key={i}
                w="95%"
              >
                <Flex>
                  {e}
                  <Spacer />
                  <IconButton
                    onClick={() => chatHub?.joinChannel(e)}
                    alignSelf="flex-end"
                    aria-label="join channel"
                    icon={<CgEnter />}
                  ></IconButton>
                </Flex>
              </ListItem>
            );
          })}
        </List>
      </Box>
    </Flex>
  );
};

export default MainPage;
