/* eslint-disable @typescript-eslint/no-non-null-assertion */
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
import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { bindActionCreators } from "redux";
import ChatHubConnection from "../../api/hubConnecton";
import { actionCreators, ChannelsReceived, ChatHub, LongOperation } from "../../state";
import AddChannel from "./AddChannel";
import { Modal, ModalContent, ModalOverlay, Spinner, HStack } from "@chakra-ui/react";


const MainPage = (): JSX.Element => {
  const dispatch = useDispatch();
  const { setHubChat } = bindActionCreators(actionCreators, dispatch);
  const channels = useSelector((state: ChannelsReceived) => {
    return state.channels;
  });

  const longOperation = useSelector(
    (state: LongOperation) => state.longOperation,
  );

  const chatHub = useSelector((state: ChatHub) => {
    return state.chatHub;
  });

  useEffect(() => {
    
    async function createHubConnection() {
      const hub = new ChatHubConnection();
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
      console.log("asdfasdf")
      createHubConnection();
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatHub])

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
                    onClick={() => chatHub!.joinChannel(e)}
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
      <Modal closeOnOverlayClick={false} isOpen={longOperation} onClose={() => { console.log("") }}>
        <ModalOverlay />
        <ModalContent alignSelf="center" alignContent="center" justifyContent="center">
          <HStack w="600" h="600" justifyContent="center">
            <Spinner size="lg" alignSelf="center" />
          </HStack>
        </ModalContent>
      </Modal>

    </Flex>

  );
};

export default MainPage;
