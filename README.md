# BDF+
Introduce
Convert custom raw eeg data to BDF+. 

## Data Structure
* The image illustrate differents between EDF and BDF.
![image](https://user-images.githubusercontent.com/53856105/167778382-2a6a7a1a-4821-4776-963c-f7e5f8d16f00.png)
* Here is the structure of the raw data
//buflen = 510;
//frame head（2byte）＋ frame num（2byte）＋ data class（1byte）＋ sample rate（2byte)＋ TRIG（1byte）＋ reserved（2byte）+ data（4×40byte）
* The raw data were saved in "D:\savebuf.dat", and temp data of BDF will saved in "D:\BDF_temp.dat".
            
## The Triggers and Annotations
In this project, there are 10 different types of triggers, and they are actived randomly during whole test. So it is possible to cause overflow, that means the number of triggers more than the number of datablock. To deal with that and keep the same samplerate, I change the duration time and number of sample automaticlly by the number of triggers. And that make program recording signals much more longer than before.
