import {
  Alert,
  AlertTitle,
  Box,
  Button,
  HStack,
  Input,
  Spinner,
} from "@chakra-ui/react";
import React from "react";
import { addChannel, ChannelResponse } from "../../api/api";

const AddChannel = (): JSX.Element => {
  const [newChannelName, setNewChannelName] = React.useState("");
  const [newChannelErrors, setNewChannelErrors] = React.useState<string[]>([]);
  const [isBusy, setIsBusy] = React.useState(false);

  async function addNewChannel(channelName: string): Promise<void> {
    setIsBusy(true);
    let response: ChannelResponse | null = null;
    await addChannel(channelName)
      .then((resp) => {
        response = resp;
      })
      .catch((err) => {
        console.log(err);
      });

    if (response !== null) {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      setNewChannelErrors(response!.errors);
    }

    setNewChannelName("");
    setIsBusy(false);
  }

  return (
    <Box m="2" borderRadius="16">
      <HStack>
        <Input
          placeholder="Add channel here..."
          value={newChannelName}
          onChange={(e) => setNewChannelName(e.target.value)}
          type="text"
        />
        {isBusy ? (
          <Spinner alignSelf="center" />
        ) : (
          <Button
            flex="1"
            w="24px"
            h="24px"
            borderRadius={25}
            type="submit"
            variant="solid"
            colorScheme="teal"
            width="full"
            onClick={() => addNewChannel(newChannelName)}
          >
            +
          </Button>
        )}
      </HStack>
      <ul style={{ listStyleType: "none" }}>
        {newChannelErrors.map((err, i) => (
          <li key={i}>
            <Alert status="error">
              <AlertTitle mr={2}>{err}</AlertTitle>
            </Alert>
          </li>
        ))}
      </ul>
    </Box>
  );
};

export default AddChannel;
