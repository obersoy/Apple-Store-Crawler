using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedLibrary.AWS
{
    public class AWSSQSHelper
    {
        ///////////////////////////////////////////////////////////////////////
        //                           Fields                                  //
        ///////////////////////////////////////////////////////////////////////

        public IAmazonSQS             queue              { get; set; }   // AMAZON simple queue service reference
        public GetQueueUrlResponse    queueurl           { get; set; }   // AMAZON queue url
        public ReceiveMessageRequest  rcvMessageRequest  { get; set; }   // AMAZON receive message request
        public ReceiveMessageResponse rcvMessageResponse { get; set; }   // AMAZON receive message response
        public DeleteMessageRequest   delMessageRequest  { get; set; }   // AMAZON delete message request

        public bool IsValid                              { get; set; }   // True when the queue is OK
                                                         
        public int ErrorCode                             { get; set; }   // Last error code
        public string ErrorMessage                       { get; set; }   // Last error message

        public const int e_Exception = -1;

        private Object deletionLock  = new Object ();

        ///////////////////////////////////////////////////////////////////////
        //                    Methods & Functions                            //
        ///////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Class constructor
        /// </summary>
        public AWSSQSHelper ()
        {
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public AWSSQSHelper(string queuename, int maxnumberofmessages, String AWSAccessKey = "", String AWSSecretKey = "")
        {
            OpenQueue(queuename, maxnumberofmessages, AWSAccessKey, AWSSecretKey);
        }

        /// <summary>
        /// The method clears the error information associated with the queue
        /// </summary>
        private void ClearErrorInfo()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// The method opens the queue
        /// </summary>
        public bool OpenQueue(string queuename, int maxnumberofmessages, String AWSAccessKey, String AWSSecretKey)
        {
            ClearErrorInfo();

            IsValid = false;

            if (!string.IsNullOrWhiteSpace(queuename))
            {
                // Checking for the need to use provided credentials instead of reading from app.Config
                if (!String.IsNullOrWhiteSpace(AWSSecretKey) && !String.IsNullOrWhiteSpace(AWSSecretKey))
                {
                    AWSCredentials awsCredentials = new BasicAWSCredentials(AWSAccessKey, AWSSecretKey);
                    queue                         = AWSClientFactory.CreateAmazonSQSClient (awsCredentials, RegionEndpoint.USEast1);
                }
                else
                {
                    queue = AWSClientFactory.CreateAmazonSQSClient (RegionEndpoint.USEast1);
                }

                try
                {
                    // Get queue url
                    GetQueueUrlRequest sqsRequest = new GetQueueUrlRequest();
                    sqsRequest.QueueName          = queuename;
                    queueurl                      = queue.GetQueueUrl(sqsRequest);

                    // Format receive messages request
                    rcvMessageRequest                     = new ReceiveMessageRequest();
                    rcvMessageRequest.QueueUrl            = queueurl.QueueUrl;
                    rcvMessageRequest.MaxNumberOfMessages = maxnumberofmessages;

                    // Format the delete messages request
                    delMessageRequest          = new DeleteMessageRequest();
                    delMessageRequest.QueueUrl = queueurl.QueueUrl;

                    IsValid = true;
                }
                catch (Exception ex)
                {
                    ErrorCode = e_Exception;
                    ErrorMessage = ex.Message;
                }
            }

            return IsValid;
        }

        /// <summary>
        /// Returns the approximate number of queued messages
        /// </summary>
        public int ApproximateNumberOfMessages()
        {
            ClearErrorInfo();

            int result = 0;
            try
            {
                GetQueueAttributesRequest attrreq = new GetQueueAttributesRequest();
                attrreq.QueueUrl                  = queueurl.QueueUrl;
                attrreq.AttributeNames.Add("ApproximateNumberOfMessages");
                GetQueueAttributesResponse attrresp = queue.GetQueueAttributes(attrreq);
                if (attrresp != null)
                    result = attrresp.ApproximateNumberOfMessages;
            }
            catch (Exception ex)
            {
                ErrorCode = e_Exception;
                ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// The method loads a one or more messages from the queue
        /// </summary>
        public bool DeQueueMessages()
        {
            ClearErrorInfo();

            bool result = false;
            try
            {
                rcvMessageResponse = queue.ReceiveMessage(rcvMessageRequest);
                result = true;
            }
            catch (Exception ex)
            {
                ErrorCode = e_Exception;
                ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// The method deletes the message from the queue
        /// </summary>
        public bool DeleteMessage(Message message)
        {
            lock (deletionLock)
            {
                ClearErrorInfo();

                bool result = false;
                try
                {
                    delMessageRequest.ReceiptHandle = message.ReceiptHandle;
                    queue.DeleteMessage(delMessageRequest);
                    result = true;
                }
                catch (Exception ex)
                {
                    ErrorCode = e_Exception;
                    ErrorMessage = ex.Message;
                }

                return result;
            }
        }

        /// <summary>
        /// Inserts a message in the queue
        /// </summary>
        public bool EnqueueMessage(string msgbody)
        {
            ClearErrorInfo();

            bool result = false;
            try
            {
                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.QueueUrl = queueurl.QueueUrl;
                sendMessageRequest.MessageBody = msgbody;
                queue.SendMessage(sendMessageRequest);
                result = true;
            }
            catch (Exception ex)
            {
                ErrorCode = e_Exception;
                ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Inserts a message in the queue and retries when an error is detected
        /// </summary>
        public bool EnqueueMessage(string msgbody, int maxretries)
        {
            // Insert domain info into queue
            bool result = false;
            int retrycount = maxretries;
            while (true)
            {
                // Try the insertion
                if (EnqueueMessage(msgbody))
                {
                    result = true;
                    break;
                }

                // Retry
                retrycount--;
                if (retrycount <= 0)
                    break;
                Thread.Sleep (new Random ().Next (500, 2000));
            }

            // Return
            return result;
        }

        public bool AnyMessageReceived ()
        {
            try
            {
                if (rcvMessageResponse == null)
                    return false;

                var messageResults = rcvMessageResponse.Messages;
  
                if (messageResults != null && messageResults.FirstOrDefault () != null)
                {
                    return true;
                }
            }
            catch 
            {
                // Nothing to do here                
            }
  
            return false;
        }

        public void ClearQueue()
        {
            do
            {
                // Dequeueing Messages
                if (!DeQueueMessages())
                {
                    // Checking for the need to abort (queue error)
                    if (!String.IsNullOrWhiteSpace (ErrorMessage))
                    {
                        return; // Abort
                    }

                    continue; // Continue in case de dequeue fails, to make sure no message will be kept in the queue
                }

                // Retrieving Message Results
                var resultMessages = rcvMessageResponse.Messages;

                // Checking for no message dequeued
                if (resultMessages.Count == 0)
                {
                    break; // Breaks loop
                }

                // Iterating over messages of the result to remove it
                foreach (Message message in resultMessages)
                {
                    // Deleting Message from Queue
                    DeleteMessage(message);
                }

            } while (true);
        }

        public void ClearQueues (List<String> queueNames, String AWSAccessKey, String AWSSecretKey)
        {
            // Iterating over queues
            foreach (string queueName in queueNames)
            {
                OpenQueue (queueName, 10, AWSAccessKey, AWSSecretKey);

                do
                {
                    // Dequeueing Messages
                    if (!DeQueueMessages())
                    {
                        continue; // Continue in case de dequeue fails, to make sure no message will be kept in the queue
                    }

                    // Retrieving Message Results
                    var resultMessages = rcvMessageResponse.Messages;

                    // Checking for no message dequeued
                    if (resultMessages.Count == 0)
                    {
                        break;
                    }

                    // Iterating over messages of the result to remove it
                    foreach (Message message in resultMessages)
                    {
                        // Deleting Message from Queue
                        DeleteMessage(message);
                    }

                } while (true);
            }
        }

        public IEnumerable<Message> GetDequeuedMessages ()
        {
            return rcvMessageResponse.Messages;
        }
    }
}
