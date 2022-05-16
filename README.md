# BDF
Introduce
Convert custom raw eeg data to BDF. 

## Data Structure
* The image illustrate differents between EDF and BDF.
![image](https://user-images.githubusercontent.com/53856105/167778382-2a6a7a1a-4821-4776-963c-f7e5f8d16f00.png)
* Here is the structure of the raw data
            //buflen = 510;
            //frame head（2字节）＋ frame num（2字节）＋ data class（1字节）＋ sample rate（2字节)＋ TRIG（1字节）＋ reserved（2字节）+ data（4×40字节）
            
## Still Updating...
If change the header and the array of sample, the project will output EDF file.
Now the siganl construction is not standard. They lake tiggers and annotations.
