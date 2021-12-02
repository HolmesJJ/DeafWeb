
p1 = ('air.mp3');
all_diff = [];
all_mean  = [];
%root = 'audio\10correctair\';
root = 'audio\10correctbaby\';
root = 'audio\20wrongair\';
%root = 'audio\00wrongbaby\';
file_list=dir(fullfile(root));
fileNum=size(file_list,1); 
for k=3:fileNum
	diff = comparison_audio(p1,[root file_list(k).name]);
    all_mean = [all_mean mean(diff)];
    all_diff = [all_diff diff];
end
disp(p1);
disp(root);
disp(all_mean);
disp(mean(all_diff));
