def normalize(time) 
  if time.include? 'a'
    result = to_min_hour time.sub 'a', ''
    if result[:hour] == 12
      result[:hour] = 0
    end
    result
  else
    result = to_min_hour time.sub 'p', ''
    result[:hour] += 12
    result
  end
end

def to_min_hour(time)
  if time.include? ':'
    split = time.split ':'
    { hour: split[0].to_i, min: split[1].to_i }
  else
    { hour: time.to_i, min: 0 }
  end
end

def to_minutes(time)
  time[:hour] * 60 + time[:min]
end

def from_minutes(minutes)
  { hour: minutes / 60, min: minutes % 60 }
end

def time_delta(start, finish)
  (to_minutes finish) - (to_minutes start)
end

sum = File.readlines('hourslog.txt')
  .select do |line|
    line.include? '-'
  end
  .inject(0) do |sum, line|
    match =  /([\d:pma]*)-([\d:pma]*)/.match(line)
    start = match[1]
    finish = match[2]

    start_time = normalize start
    finish_time = normalize finish
    sum + (time_delta start_time, finish_time)
  end

puts from_minutes sum
