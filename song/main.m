addpath(genpath('voicebox')) 
[d1,sr] = audioread('baby.mp3');
[d2,sr] = audioread('audio\10correctbaby\Voice 007.m4a');
%soundsc(d1,sr)
%soundsc(d2,sr)
t =  zeros(2048,1);
d1 = d1(:,1);
d1 = [t ;d1; t];
d2 = d2(:,1);
d2 = [t ;d2; t];

%figure(1);
%subplot(411)
%plot(d1)
%subplot(412)
%plot(d2)

d1vad=vad(d1);
d2vad=vad(d2);

d1f= d1vad(1,1);
d1vad_size = size(d1vad);
d1e= d1vad(d1vad_size(1),2);

d2f= d2vad(1,1);
d2vad_size = size(d2vad);
d2e= d2vad(d2vad_size(1),2);

ccc1 = mfcc(d1,sr); %ccc1 = mfcc(d1(d1f:d1e,:));
ccc2 = mfcc(d2,sr);

ccc1(isnan(ccc1)) = 0;
ccc2(isnan(ccc2)) = 0;

[dist,diff] = dtw(ccc1,ccc2);

d1vad_1=  d1vad;
diff_size = size(diff);
d1_size = size(d1);
t = diff_size(1) / double(d1e-d1f);
d1vad_1= double(d1vad_1) * (diff_size(2) / double(d1_size(1)));
d1vad_1= int32(d1vad_1);

figure(2);

f= 1:length(diff);
subplot(411)


plot(f,diff);
axis([1 length(diff) 0 max(diff)])
line([1 length(diff)], [8 8], 'Color', 'red');
line([1 length(diff)], [6 6], 'Color', 'green');
d1vad_1= [ d1vad_1;[0 0]];
B=reshape(d1vad_1,[],1);
line([B B], [0 max(diff)], 'Color', 'red');
subplot(412)


%fs=1000;
%c = round(sr/fs);
%x=resample(d1x,1,c);
plot(d1)
%axis([d1f d1e -1 1])
ylabel('Speech');
d1vad= [ d1vad;[0 0]];
B=reshape(d1vad,[],1);
line([B B], [-1 1], 'Color', 'red');

subplot(413)

plot(d2)
%axis([d2f d2e -1 1])
ylabel('Speech');
d2vad= [ d2vad;[0 0]];
B=reshape(d2vad,[],1);
line([B B], [-1 1], 'Color', 'red');
