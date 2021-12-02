function [diff,d1,d2,step1,step2,step_diff] = comparison_audio (d1_path,d2_path)
% 此处显示有关此函数的摘要
%   此处显示详细说明
[d1,sr] = audioread(d1_path);
[d2,sr] = audioread(d2_path);

t =  zeros(2048,1);
d1 = d1(:,1);
d1 = [t ;d1; t];
d2 = d2(:,1);
d2 = [t ;d2; t];

d1vad=vad(d1);
%d2vad=vad(d2);

%d1f= d1vad(1,1);
%d1vad_size = size(d1vad);
%d1e= d1vad(d1vad_size(1),2);

%d2f= d2vad(1,1);
%d2vad_size = size(d2vad);
%d2e= d2vad(d2vad_size(1),2);

ccc1 = mfcc(d1,sr);
ccc2 = mfcc(d2,sr);

ccc1(isnan(ccc1)) = 0;
ccc2(isnan(ccc2)) = 0;

[~,diff] = dtw(ccc1,ccc2);

d1vad_1=  d1vad;
diff_size = size(diff);
d1_size = size(d1);
d1vad_1= double(d1vad_1) * (diff_size(2) / double(d1_size(1)));
d1vad_1= int32(d1vad_1);

fs=1000;
c = round(sr/fs);
step1=vad(d1)/c;
step2=vad(d2)/c;
step_diff=d1vad_1;
step1=reshape(step1,[],1);
step2=reshape(step2,[],1);
step_diff=reshape(step_diff,[],1);
d1=resample(d1,1,c);
d2=resample(d2,1,c);
end

