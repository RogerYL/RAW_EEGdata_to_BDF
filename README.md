# BDF
Introduce
Convert custom raw eeg data to BDF. 

## Data Structure
* The image illustrate differents between EDF and BDF.
![image](https://user-images.githubusercontent.com/53856105/167778382-2a6a7a1a-4821-4776-963c-f7e5f8d16f00.png)
* Here is the structure of the raw data
            * **//buflen = 510;
            * **//frame head（2byte）＋ frame num（2byte）＋ data class（1byte）＋ sample rate（2byte)＋ TRIG（1byte）＋ reserved（2byte）+ data（4×40byte）
            
## Still Updating...
If change the header and the array of sample, the project will output EDF file.
Now the siganl construction is not standard. They lake tiggers and annotations.
