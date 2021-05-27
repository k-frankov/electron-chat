import { HStack, Spacer, VStack } from "@chakra-ui/layout";
import {
  Badge,
  Box,
  Flex,
  Heading,
  IconButton,
  Input,
  List,
  ListItem,
  useToast,
} from "@chakra-ui/react";
import { MdSend } from "react-icons/md";
import { BiShare } from "react-icons/bi";
import React from "react";
import { ChannelJoined, ChatHub, AuthenticatedUser } from "../../state";
import { useSelector } from "react-redux";
import { ChatMessage } from "../../state/models/chatMessage";
import { shareFile } from "../../api/api";
const { ipcRenderer } = window.require('electron');

const Channel = (): JSX.Element => {
  const chatHub = useSelector((state: ChatHub) => {
    return state.chatHub;
  });
  const [channelUsers, setChannelUsers] = React.useState<string[]>([]);
  const [channelMessages, setChannelMessages] = React.useState<ChatMessage[]>(
    [],
  );
  const [newMessage, setNewMessage] = React.useState<string>("");
  
  const toast = useToast();
  React.useEffect(() => {
    ipcRenderer.on('fileData', async (event, data, fileName) => {
      const errors = await shareFile(data, fileName);
      if (errors.length > 0) {
        for (const shareError of errors) {
          toast({
            title: "Something wrong happened",
            description: shareError,
            status: "error",
            duration: 9000,
            isClosable: true,
          });
        }
      }
    });
  }, [toast]);

  if (chatHub !== null) {
    chatHub.channelUsersCallback = (users: string[]): void => {
      setChannelUsers(users);
    };

    chatHub.channelOldMessagesCallback = (messages: ChatMessage[]): void => {
      console.log(messages)
      setChannelMessages(messages);
    };

    chatHub.channelNewMessageCallback = (message: ChatMessage): void => {
      setChannelMessages([...channelMessages, message]);
    };
  }

  const currentUser = useSelector((state: AuthenticatedUser) => {
    return state.authenticatedUser;
  });

  const currentChannelName: string = useSelector((state: ChannelJoined) => {
    return state.channelJoined;
  });

  return (
    <HStack w="100%" h="100%" padding="1">
      <VStack w="80%" h="100%">
        <VStack
          w="100%"
          h="100%"
          flex="1"
          style={{ maxHeight: "calc(100vh - 155px)" }}
          border="1px"
          borderRadius="4"
        >
          <List
            spacing="2"
            overflow="auto"
            width="100%"
            height="100%"
            padding="2"
          >
            {channelMessages.map((message: ChatMessage, key) => {
              if (message.userName === currentUser?.userName) {
                return (
                  <ListItem key={key} justifySelf="end">
                    <Flex>
                      <Spacer />
                      <VStack>
                        <Box width="100%" textAlign="end">
                          <Badge variant="solid" colorScheme="green">
                            {message.userName}:
                          </Badge>{" "}
                          {message.message}
                        </Box>
                        <Box width="100%" textAlign="end">
                          at: {message.messageTime}
                        </Box>
                      </VStack>
                    </Flex>
                  </ListItem>
                );
              } else {
                return (
                  <ListItem key={key} width="90%">
                    <Flex>
                      <VStack>
                        <Box width="100%" textAlign="start">
                          <Badge variant="solid" colorScheme="purple">
                            {message.userName}:
                          </Badge>{" "}
                          {message.message}
                        </Box>
                        <Box width="100%" textAlign="start">
                          at: {message.messageTime}
                        </Box>
                      </VStack>
                    </Flex>
                  </ListItem>
                );
              }
            })}
          </List>
        </VStack>
        <Box h="60px" w="100%" border="1px" borderRadius="4" padding="9px">
          <HStack>
            <Input
              placeholder="You can type here..."
              w="100%"
              value={newMessage}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  chatHub?.sendMessageToChannel(currentChannelName, newMessage);
                  setNewMessage("");
                }
              }}
              onChange={(e) => {
                setNewMessage(e.target.value);
              }}
            />
            <Spacer />
            <IconButton
              icon={<MdSend />}
              aria-label="send"
              onClick={() => {
                chatHub?.sendMessageToChannel(currentChannelName, newMessage);
                setNewMessage("");
              }}
            />
            <IconButton icon={<BiShare />} aria-label="send" onClick={() => {
              ipcRenderer.send("openFile");
            }} />
          </HStack>
        </Box>
      </VStack>
      <VStack h="100%" w="20%" border="1px" borderRadius="4" padding="2">
        <Heading color="teal.500" size="md">
          People
        </Heading>
        <List
          width="100%"
          border="1px"
          borderRadius="4"
          marginRight="1"
          marginLeft="1"
          paddingTop="1"
          paddingLeft="4"
          paddingBottom="1"
          spacing={3}
          overflow="auto"
          style={{ maxHeight: "calc(100vh - 190px)" }}
        >
          {channelUsers.map((user, i) => {
            return (
              <ListItem key={i} w="100%">
                {user}
              </ListItem>
            );
          })}
        </List>
      </VStack>
    </HStack>
  );
};

export default Channel;
